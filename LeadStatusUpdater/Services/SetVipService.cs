using LeadStatusUpdater.Common;
using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Extensions;
using LeadStatusUpdater.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Services
{
    public class SetVipService : ISetVipService
    {
        private IRequestsSender _requests;
        private IConverterService _converter;
        private readonly EmailPublisher _emailPublisher;
        private string _adminToken;


        public SetVipService(IRequestsSender sender,
            IConverterService converter,
            EmailPublisher emailPublisher)
        {
            _requests = sender;
            _converter = converter;
            _emailPublisher = emailPublisher;
        }

        public void Process()
        {
            _adminToken = _requests.GetAdminToken();

            var leads = new List<LeadOutputModel>();
            var leadsToChangeStatusList = new List<LeadIdAndRoleInputModel>();
            var leadsToLogAndEmail = new List<LeadOutputModel>();
            int lastLeadId = 0;
            int leadsCount = 0;

            do
            {
                leads = _requests.GetRegularAndVipLeads(_adminToken, lastLeadId);
                leadsCount = leads.Count;

                if (leads != null && leadsCount > 0)
                {
                    Log.Information($"{leadsCount} leads were retrieved from database");

                    leads.ForEach(lead =>
                    {
                        var newRole = CheckOneLead(lead) ? Role.Vip : Role.Regular;
                        if (lead.Role != newRole)
                        {
                            leadsToChangeStatusList.Add(new LeadIdAndRoleInputModel { Id = lead.Id, Role = newRole });
                            lead.Role = newRole;
                        }
                    });

                    _requests.ChangeStatus(leadsToChangeStatusList, _adminToken); 

                    leadsToLogAndEmail.AddRange(leads.Where(l => leadsToChangeStatusList.Any(c => l.Id == c.Id)));

                    lastLeadId = leads.Last().Id;
                }
            }
            while (leads != null && leadsCount > 0);

            Log.Information($"All leads were processed");

            foreach (var lead in leadsToLogAndEmail) //change to async
            {
                string logMessage = lead.Role == Role.Vip ? $"{LogMessages.VipStatusGiven} " : $"{LogMessages.VipStatusTaken} ";
                logMessage = string.Format(logMessage, lead.Id, lead.LastName, lead.FirstName, lead.Patronymic, lead.Email);
                Log.Information(logMessage);
            }

            foreach (var lead in leadsToLogAndEmail) //change to async
            {
                string logMessage = lead.Role == Role.Vip ? $"{LogMessages.VipStatusGiven} " : $"{LogMessages.VipStatusTaken} ";
                logMessage = string.Format(logMessage, lead.Id, lead.LastName, lead.FirstName, lead.Patronymic, lead.Email);

                Task.Run(() => _emailPublisher.PublishEmail(new EmailModel
                {
                    Subject = EmailMessage.StatusChangedSubject,
                    Body = String.Format(EmailMessage.StatusChangedBody, lead.Role),
                    MailAddresses = lead.Email
                })).Wait();
            }
        }

        public bool CheckOneLead(LeadOutputModel lead)
        {
            return (
                CheckBirthdayCondition(lead)
                ||CheckOperationsCondition(lead) 
                ||CheckBalanceCondition(lead)
                );
        }


        public bool CheckOperationsCondition(LeadOutputModel lead)
        {
            int transactionsCount = 0;
            foreach (var account in lead.Accounts)
            {
                var transactions = this.GetTransactionsByPeriod(_requests, _adminToken, Const.PERIOD_FOR_CHECK_TRANSACTIONS_FOR_VIP, account.Id)
                    .FirstOrDefault();

                if (transactions.Transactions != null && transactions.Transactions.Count > 0)
                {
                    transactionsCount += transactions.Transactions.
                    Where(t => t.TransactionType == TransactionType.Deposit).Count();
                }
                if(transactions.Transfers != null && transactions.Transfers.Count > 0)
                {
                    transactionsCount += transactions.Transfers.Count();
                }
                
                if (transactionsCount > Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP) return true;
            }
            return false;
        }

        public bool CheckBalanceCondition(LeadOutputModel lead)
        {
            decimal sum = 0;

            foreach (var account in lead.Accounts)
            {
                var transactions = this.GetTransactionsByPeriod(_requests, _adminToken, Const.PERIOD_FOR_CHECK_SUM_FOR_VIP, account.Id)
                    .FirstOrDefault();

                if (transactions.Transactions != null && transactions.Transactions.Count > 0)
                {
                    transactions.Transactions.ForEach( transaction =>
                        sum += transaction.Currency == Currency.RUB ?
                            transaction.Amount : _converter.ConvertAmount(transaction.Currency.ToString(), Currency.RUB.ToString(), transaction.Amount)
                        );
                }
            }            

            return sum > Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP;
        }

        public bool CheckBirthdayCondition(LeadOutputModel lead)
        {
            var leadBirthDate = Convert.ToDateTime(lead.BirthDate);
            var leadBirthdayInCurrentYear = new DateTime(DateTime.Now.Year, leadBirthDate.Month, leadBirthDate.Day);

            if (leadBirthdayInCurrentYear.Date <= DateTime.Today.Date
                && leadBirthdayInCurrentYear.Date >= DateTime.Today.AddDays(-Const.COUNT_DAY_AFTER_BDAY_FOR_VIP).Date)
            {
                if (leadBirthDate.Day == DateTime.Now.Day
                && leadBirthDate.Month == DateTime.Now.Month)
                {
                    Task.Run(() => _emailPublisher.PublishEmail(new EmailModel
                    {
                        Subject = EmailMessage.HappyBirthdaySubject,
                        Body = String.Format(EmailMessage.HappyrthdayBody, lead.LastName, lead.FirstName),
                        MailAddresses = lead.Email
                    })).Wait();

                    return true;
                }
                return true;
            }
            return false;
        }

    }
}
