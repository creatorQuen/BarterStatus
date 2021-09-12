using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace LeadStatusUpdater.Services
{
    public class SetVipService : ISetVipService
    {
        private IRequestsSender _requests;

        public SetVipService(IRequestsSender sender)
        {
            _requests = sender;
        }

        public void Process()
        {
            var leads = _requests.GetAllLeads(); //get leads by fi;ters (roles 1 2)
            foreach (var lead in leads)
            {
                if (CheckOneLead(lead))
                {
                    _requests.ChangeStatus(lead.Id, Role.Vip);
                }
                else
                {
                    _requests.ChangeStatus(lead.Id, Role.Regular);
                }
            }
        }

        public bool CheckOneLead(LeadShortModel lead)
        {
            return (//CheckBirthdayCondition(lead.BirthDate) ||
                CheckOperationsCondition(lead) ||
                CheckBalanceCondition(lead);
        }


        public bool CheckOperationsCondition(LeadShortModel lead)
        {
            int transactionsCount = 0;
            foreach (var accountId in lead.Accounts)
            {
                TimeBasedAcquisitionInputModel period = new TimeBasedAcquisitionInputModel
                {
                    To = DateTime.Now.ToString(),
                    From = DateTime.Now.AddDays(-Const.PERIOD_FOR_CHECK_TRANSACTIONS_FOR_VIP).ToString(),
                    AccountId = accountId
                };
                var accountsWithTransactions = _requests.GetTransactionsByPeriod(period);

                transactionsCount += accountsWithTransactions.FirstOrDefault().Transactions.
                    Where(t => t.TransactionType == TransactionType.Deposit).Count();
                transactionsCount += accountsWithTransactions.FirstOrDefault().Transfers.Count();

                if (transactionsCount > Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP) return true;
            }
            return false;
        }

        public bool CheckBalanceCondition(LeadShortModel lead)
        {
            decimal sumDeposit = 0;
            decimal sumWithdraw = 0;
            TimeBasedAcquisitionInputModel model = new TimeBasedAcquisitionInputModel
            {
                To = DateTime.Now.ToString(),
                From = DateTime.Now.AddDays(-Const.PERIOD_FOR_CHECK_SUM_FOR_VIP).ToString(),
                AccountId = lead.Id
            };
            var transactions = _requests.GetTransactionsByPeriod(model);

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
            if (Math.Abs(sumWithdraw) > sumDeposit + Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP) return true;
            return false;
        }

        public bool CheckBirthdayCondition(string bDay)
        {
            bDay = bDay.Substring(5, 5);
            var date = DateTime.ParseExact(bDay, "MM.dd", null);
            if (date <= DateTime.Now && date.AddDays(Const.COUNT_DAY_AFTER_BDAY_FOR_VIP) > DateTime.Now) return true;
            return false;
        }

    }
}
