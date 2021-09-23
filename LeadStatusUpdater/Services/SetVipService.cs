using Dasync.Collections;
using LeadStatusUpdater.Common;
using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Services
{
    public class SetVipService : ISetVipService
    {
        public static string AdminToken;
        private IRequestsSender _requests;
        private IConverterService _converter;
        private readonly RabbitMqPublisher _emailPublisher;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;
        private readonly DateTime _fromDateCheckBirthday;
        private readonly DateTime _toDateCheckBirthday;
        private readonly DateTime _fromDateCheckBalance;


        public SetVipService(IRequestsSender sender,
            IConverterService converter,
            RabbitMqPublisher emailPublisher)
        {
            _requests = sender;
            _converter = converter;
            _emailPublisher = emailPublisher;
            _toDateCheckBirthday = DateTime.Today.Date;
            _fromDateCheckBirthday = _toDateCheckBirthday.AddDays(-Const.COUNT_DAY_AFTER_BDAY_FOR_VIP).Date;
            _fromDateCheckBalance = DateTime.Now.AddMonths(-1);
        }

        public void Process(object obj)
        {
            AdminToken = _requests.GetAdminToken();

            var leads = new List<LeadOutputModel>();
            var leadsToChangeStatusList = new List<LeadIdAndRoleInputModel>();
            var leadsToLogAndEmail = new List<LeadOutputModel>();
            int lastLeadId = 0;
            int totalLeadsCount = 0;
            int batchCount = 0;

            do
            {
                leads = _requests.GetRegularAndVipLeads(lastLeadId);
                batchCount = leads.Count;
                totalLeadsCount += batchCount;

                if (leads != null && batchCount > 0)
                {
                    Log.Information($"{batchCount} leads were retrieved from database");

                    leads
                          .AsParallel()
                          .ForAll(lead =>
                          {
                              var newRole = Task.Run(() => CheckOneLead(lead)).Result ? Role.Vip : Role.Regular;
                              if (lead.Role != newRole)
                              {
                                  leadsToChangeStatusList.Add(new LeadIdAndRoleInputModel { Id = lead.Id, Role = newRole });
                                  lead.Role = newRole;
                              }
                          });

                    _requests.ChangeStatus(leadsToChangeStatusList); 

                    leadsToLogAndEmail.AddRange(leads.Where(l => leadsToChangeStatusList.Any(c => l.Id == c.Id)));

                    lastLeadId = leads.Last().Id;
                }
            }
            while (leads != null && batchCount > 0);

            Log.Information($"{totalLeadsCount} leads were processed.");
            Log.Information($"{leadsToChangeStatusList.Count} leads were updated.");

            leadsToLogAndEmail.
                AsParallel()
                .ForAll(lead => LogAndPublishEmails(lead));
        }

        public async Task<bool> CheckOneLead(LeadOutputModel lead)
        {
            if (CheckBirthdayCondition(lead).Result == true) return true;

            if (ConverterService.RatesModel == null) throw new Exception(LogMessages.RatesNotProvided);
            var transactions = _requests
                                    .GetTransactionsByPeriod((from acc in lead.Accounts select acc.Id)
                                    .ToList());
            if (transactions == null || transactions.Count == 0) return false;

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;
            var tasks = new List<Task<bool>> 
            {
                CheckOperationsCondition(transactions),
                CheckBalanceCondition(transactions)
            };

            while (tasks.Any())
            {
                var t = await Task.WhenAny(tasks);
                if (t.Result == true) return true;
                tasks.Remove(t);
            }
            return false;
        }

        public async Task<bool> CheckOperationsCondition(List<TransactionOutputModel> transactions)
        {
            if (transactions.
                    Where(t => t.TransactionType == TransactionType.Deposit
                    || t.TransactionType == TransactionType.Transfer).Count() > Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP)
            {
                _cancelTokenSource.Cancel();
                return true;
            }
            return false;
        }

        public async Task<bool> CheckBalanceCondition(List<TransactionOutputModel> transactions)
        {
            decimal sum = 0;

            var transactionsToCheck = transactions.Where(t => (t.TransactionType == TransactionType.Deposit
                    || t.TransactionType == TransactionType.Withdraw) && (t.Date >= _fromDateCheckBalance)).ToList();

            foreach (var transaction in transactionsToCheck)
            {
                if (_cancelToken.IsCancellationRequested) return false;
                sum += transaction.Currency == Currency.RUB.ToString() ?
                    transaction.Amount : _converter.ConvertAmount(transaction.Currency, Currency.RUB.ToString(), transaction.Amount);
            }

            if(sum > Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP)
            {
                _cancelTokenSource.Cancel();
                return true;
            }
            return false;
        }

        public async Task<bool> CheckBirthdayCondition(LeadOutputModel lead)
        {
            var leadBirthDate = Convert.ToDateTime(lead.BirthDate);

            if ((leadBirthDate.Date.Month > _fromDateCheckBirthday.Month
                || (leadBirthDate.Date.Month == _fromDateCheckBirthday.Month && leadBirthDate.Date.Day >= _fromDateCheckBirthday.Day))
                &&
                (leadBirthDate.Date.Month < _toDateCheckBirthday.Month
                || (leadBirthDate.Date.Month == _toDateCheckBirthday.Month && leadBirthDate.Date.Day <= _toDateCheckBirthday.Day)))
            {
                if (leadBirthDate.Day == DateTime.Now.Day
                && leadBirthDate.Month == DateTime.Now.Month)
                {
                    await _emailPublisher
                    .PublishMessage(EmailMessage.GetBirthdayEmail(lead));
                    return true;
                }
                return true;
            }
            return false;
        }

        private void LogAndPublishEmails(LeadOutputModel lead)
        {
            LogStatusChanged(lead);

            Task.Run(() => _emailPublisher
                .PublishMessage(EmailMessage.GetStatusChangedEmail(lead))).Wait();
        }

        private void LogStatusChanged(LeadOutputModel lead)
        {
            string logMessage = lead.Role == Role.Vip ? $"{LogMessages.VipStatusGiven} " : $"{LogMessages.VipStatusTaken} ";
            logMessage = string.Format(logMessage, lead.Id, lead.LastName, lead.FirstName, lead.Patronymic, lead.Email);
            Log.Information(logMessage);
        }
    }
}
