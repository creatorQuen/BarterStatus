using LeadStatusUpdater.Common;
using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
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
        private const string _dateFormatWithMinutesAndSeconds = "dd.MM.yyyy HH:mm";
        private string _adminToken;


        public SetVipService(IRequestsSender sender,
            IConverterService converter,
            EmailPublisher emailPublisher)
        {
            _requests = sender;
            _converter = converter;
            _emailPublisher = emailPublisher;
        }

        public async void Process(object obj)
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

                    leads.ForEach(async lead =>
                    {
                        var newRole = await CheckOneLead(lead) ? Role.Vip : Role.Regular;
                        if (lead.Role != newRole)
                        {
                            leadsToChangeStatusList.Add(new LeadIdAndRoleInputModel { Id = lead.Id, Role = newRole });
                        }
                    });

                    _requests.ChangeStatus(leadsToChangeStatusList, _adminToken); 

                    leadsToLogAndEmail.AddRange(leads.Where(l => leadsToChangeStatusList.Any(c => l.Id == c.Id)));

                    lastLeadId = leads.Last().Id;
                }
            }
            while (leads != null && leadsCount > 0);

            Log.Information($"All leads were processed");
            foreach (var lead in leadsToLogAndEmail)
            {
                string logMessage = lead.Role == Role.Vip ? $"{LogMessages.VipStatusGiven} " : $"{LogMessages.VipStatusTaken} ";
                logMessage = string.Format(logMessage, lead.Id, lead.LastName, lead.FirstName, lead.Patronymic, lead.Email);
                Log.Information(logMessage);

                await _emailPublisher.PublishEmail(new EmailModel 
                { 
                    Subject = "Status changed", //add to consts
                    Body = $"You status has been changed to {lead.Role}",
                    MailAddresses = lead.Email
                });
            }
        }

        public async Task <bool> CheckOneLead(LeadOutputModel lead)
        {
            return (
                await CheckBirthdayCondition(lead)
                //||CheckOperationsCondition(lead) 
                //||CheckBalanceCondition(lead)
                );
        }


        public bool CheckOperationsCondition(LeadOutputModel lead)
        {
            int transactionsCount = 0;
            foreach (var account in lead.Accounts)
            {
                TimeBasedAcquisitionInputModel period = new TimeBasedAcquisitionInputModel
                {
                    To = DateTime.Now.ToString(_dateFormatWithMinutesAndSeconds),
                    From = DateTime.Now.AddDays(-Const.PERIOD_FOR_CHECK_TRANSACTIONS_FOR_VIP).ToString(_dateFormatWithMinutesAndSeconds),
                    AccountId = account.Id
                };

                var transactions = _requests.GetTransactionsByPeriod(period, _adminToken).FirstOrDefault();

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

            TimeBasedAcquisitionInputModel period = new TimeBasedAcquisitionInputModel
            {
                To = DateTime.Now.ToString(_dateFormatWithMinutesAndSeconds),
                From = DateTime.Now.AddDays(-Const.PERIOD_FOR_CHECK_SUM_FOR_VIP).ToString(_dateFormatWithMinutesAndSeconds),
                AccountId = lead.Id
            };

            var transactions = _requests.GetTransactionsByPeriod(period, _adminToken).FirstOrDefault();

            if(transactions.Transactions != null && transactions.Transactions.Count > 0)
            {
                foreach (var transaction in transactions.Transactions)
                {
                    if (transaction.TransactionType == TransactionType.Deposit)
                    {
                        if (transaction.Currency == Currency.RUB)
                        {
                            sum += transaction.Amount;
                        }
                        else
                        {
                            var convertedAmount = _converter.ConvertAmount(transaction.Currency.ToString(), Currency.RUB.ToString(), transaction.Amount);
                            sum += convertedAmount;
                        }
                    }
                    else if (transaction.TransactionType == TransactionType.Withdraw)
                    {
                        if (transaction.Currency == Currency.RUB)
                        {
                            sum -= transaction.Amount;
                        }
                        else
                        {
                            var convertedAmount = _converter.ConvertAmount(transaction.Currency.ToString(), Currency.RUB.ToString(), transaction.Amount);
                            sum -= convertedAmount;
                        }
                    }
                }
            }

            return (sum > Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP);
        }

        public async Task<bool> CheckBirthdayCondition(LeadOutputModel lead)
        {
            var leadBirthDate = Convert.ToDateTime(lead.BirthDate);
            var leadBirthdayInCurrentYear = new DateTime(DateTime.Now.Year, leadBirthDate.Month, leadBirthDate.Day);

            if (leadBirthdayInCurrentYear.Date <= DateTime.Today.Date
                && leadBirthdayInCurrentYear.Date >= DateTime.Today.AddDays(-Const.COUNT_DAY_AFTER_BDAY_FOR_VIP).Date)
            {
                if (leadBirthDate.Day == DateTime.Now.Day
                && leadBirthDate.Month == DateTime.Now.Month)
                {
                    await _emailPublisher.PublishEmail(new EmailModel
                    {
                        Subject = "Happy birthday",//add to consts
                        Body = $"Dear, {lead.LastName} {lead.FirstName}! Happy Birthday!",
                        MailAddresses = lead.Email
                    });
                    
                    return true;
                }
                return true;
            }
            return false;
        }

    }
}
