using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeadStatusUpdater.Services
{
    public class SetVipService : ISetVipService
    {
        private IRequestsSender _requests;
        private const string _dateFormat = "MM.dd";
        private const string _dateFormatWithMinutesAndSeconds = "dd.MM.yyyy HH:mm";
        private string _adminToken;

        public SetVipService(IRequestsSender sender)
        {
            _requests = sender;
        }

        public void Process()
        {
            _adminToken = _requests.GetAdminToken();

            var leads = new List<LeadOutputModel>();
            int cursor = 0;
            do
            {
                leads = _requests.GetRegularAndVipLeads(_adminToken, cursor);

                leads.ForEach(lead =>
                {
                    var newRole = CheckOneLead(lead) ? Role.Vip : Role.Regular;
                    if (lead.Role != newRole)
                    {
                        _requests.ChangeStatus(lead.Id, newRole, _adminToken); //change
                        string logMessage = newRole == Role.Vip ? $"{LogMessages.VipStatusGiven} " : $"{LogMessages.VipStatusTaken} ";
                        logMessage = string.Format(logMessage, lead.Id, lead.LastName, lead.FirstName, lead.Patronymic, lead.Email);
                        Log.Information(logMessage);
                    }
                });

                if (leads != null)
                {
                    cursor = leads.Last().Id;
                }
            }
            while (leads != null);
        }

        public bool CheckOneLead(LeadOutputModel lead)
        {
            return (
                CheckBirthdayCondition(lead) //||
                //CheckOperationsCondition(lead) 
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

                if(transactions.Transactions.Count > 0)
                {
                    transactionsCount += transactions.Transactions.
                    Where(t => t.TransactionType == TransactionType.Deposit).Count();
                }
                if(transactions.Transfers.Count > 0)
                {
                    transactionsCount += transactions.Transfers.Count();
                }
                
                if (transactionsCount > Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP) return true;
            }
            return false;
        }

        public bool CheckBalanceCondition(LeadOutputModel lead)
        {
            decimal sumDeposit = 0;
            decimal sumWithdraw = 0;
            TimeBasedAcquisitionInputModel model = new TimeBasedAcquisitionInputModel
            {
                To = DateTime.Now.ToString(),
                From = DateTime.Now.AddDays(-Const.PERIOD_FOR_CHECK_SUM_FOR_VIP).ToString(),
                AccountId = lead.Id
            };
            var transactions = _requests.GetTransactionsByPeriod(model, _adminToken);

            //foreach (var tr in transactions)
            //{
            //    if (tr.TransactionType == TransactionType.Deposit)
            //    {
            //        sumDeposit += tr.Amount;
            //    }
            //    if (tr.TransactionType == TransactionType.Withdraw)
            //    {
            //        sumWithdraw += tr.Amount;
            //    }
            //}

            return (Math.Abs(sumWithdraw) > sumDeposit + Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP);
        }

        public bool CheckBirthdayCondition(LeadOutputModel lead)
        {
            var leadBirthDate = Convert.ToDateTime(lead.BirthDate);
            if(leadBirthDate.Day == DateTime.Now.Day 
                && leadBirthDate.Month == DateTime.Now.Month)
            {
                //send email
                return true;
            }

            if (leadBirthDate.Day <= DateTime.Now.Day 
                && leadBirthDate.Day >= DateTime.Now.AddDays(-Const.COUNT_DAY_AFTER_BDAY_FOR_VIP).Day)
            {
                return true;
            }

            return false;
        }

    }
}
