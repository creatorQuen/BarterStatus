using LeadStatusUpdater.Common;
using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Extensions;
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


        public SetVipService(IRequestsSender sender,
            IConverterService converter,
            RabbitMqPublisher emailPublisher)
        {
            _requests = sender;
            _converter = converter;
            _emailPublisher = emailPublisher;
            var datet = new DateTime(2021, 3, 5);
            _fromDateCheckBirthday = datet.AddDays(-Const.COUNT_DAY_AFTER_BDAY_FOR_VIP).Date;
            _toDateCheckBirthday = datet.Date;
        }

        public async Task Process(object obj)
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

                    leads.ForEach(lead =>
                    {
                        var newRole = Task.Run(() => CheckOneLead(lead)).Result ? Role.Vip : Role.Regular;
                        if (lead.Role != newRole)
                        {
                            leadsToChangeStatusList.Add(new LeadIdAndRoleInputModel { Id = lead.Id, Role = newRole });
                            lead.Role = newRole;
                        }
                    });

                    //leads
                    //    .AsParallel()
                    //    //.WithDegreeOfParallelism(Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)))
                    //    .ForAll(lead => {
                    //        var newRole = CheckOneLead(lead) ? Role.Vip : Role.Regular;
                    //        if (lead.Role != newRole)
                    //        {
                    //            leadsToChangeStatusList.Add(new LeadIdAndRoleInputModel { Id = lead.Id, Role = newRole });
                    //            lead.Role = newRole;
                    //        }
                    //    });

                    _requests.ChangeStatus(leadsToChangeStatusList); 

                    leadsToLogAndEmail.AddRange(leads.Where(l => leadsToChangeStatusList.Any(c => l.Id == c.Id)));

                    lastLeadId = leads.Last().Id;
                    Log.Information("batch done");
                }
            }
            while (leads != null && batchCount > 0);

            Log.Information($"{totalLeadsCount} leads were processed.");

            leadsToLogAndEmail.
                AsParallel()
                .ForAll(lead => LogAndPublishEmails(lead));
        }

        public async Task<bool> CheckOneLead(LeadOutputModel lead)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;
            if (CheckBirthdayCondition(lead) == true) return true;

            var tasks = new List<Task<bool>>();
            tasks.Add(CheckOperationsCondition(lead));
            tasks.Add(CheckBalanceCondition(lead));

            while (tasks.Any())
            {
                var t = await Task.WhenAny(tasks);
                if (t.Result == true)
                {
                    return true;
                }
                tasks.Remove(t);
            }
            return false;
        }

        public async Task<bool> CheckOperationsCondition(LeadOutputModel lead)
        {
            int transactionsCount = 0;
            foreach (var account in lead.Accounts)
            {
                if (_cancelToken.IsCancellationRequested) return false;

                var transactions = this.GetTransactionsByPeriod(_requests, Const.PERIOD_FOR_CHECK_TRANSACTIONS_FOR_VIP, account.Id)
                    .FirstOrDefault();

                if (transactions.Transactions != null && transactions.Transactions.Count > 0)
                {
                    transactionsCount += transactions.Transactions.
                    Where(t => t.TransactionType == TransactionType.Deposit).Count();
                }
                if (transactions.Transfers != null && transactions.Transfers.Count > 0)
                {
                    transactionsCount += transactions.Transfers.Count();
                }

                if (transactionsCount > Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP)
                {
                    _cancelTokenSource.Cancel();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CheckBalanceCondition(LeadOutputModel lead)
        {
            decimal sum = 0;

            foreach (var account in lead.Accounts)
            {
                if (_cancelToken.IsCancellationRequested) return false;
                var transactions = this.GetTransactionsByPeriod(_requests, Const.PERIOD_FOR_CHECK_SUM_FOR_VIP, account.Id)
                    .FirstOrDefault();
                if (_cancelToken.IsCancellationRequested) return false;
                if (transactions.Transactions != null && transactions.Transactions.Count > 0)
                {
                    foreach (var transaction in transactions.Transactions)
                    {
                        if (_cancelToken.IsCancellationRequested) return false;
                        sum += transaction.Currency == Currency.RUB ?
                            transaction.Amount : _converter.ConvertAmount(transaction.Currency.ToString(), Currency.RUB.ToString(), transaction.Amount);
                    }
                }
            }
            _cancelTokenSource.Cancel();
            return sum > Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP;
        }

        public bool CheckBirthdayCondition(LeadOutputModel lead)
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
                    Task.Run(() => _emailPublisher
                    .PublishMessage(EmailMessage.GetBirthdayEmail(lead))).Wait();
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
