using Microsoft.Extensions.Options;
using BarterStatus.Models;
using BarterStatus.Settings;
using BarterStatus.Constants;
using System;
using System.Collections.Generic;
using BarterStatus.Enums;

namespace BarterStatus.Services
{
    public class SetVipService : ISetVipService
    {
        private IRequestsSender _requests;

        public SetVipService(IOptions<AppSettings> options, IRequestsSender sender)
        {
            _requests = sender;
        }

        public void Process()
        {
            var leads = _requests.GetAllLeads();
            foreach(var lead in leads)
            {
                _requests.SetVipStatus(lead.Id, CheckOneLead(lead));
            }
        }

        public bool CheckOneLead(LeadShortModel lead)
        {
            return (CheckBirthdayCondition(lead.BirthDate) || CheckOperationsCondition(lead) || CheckBalanceCondition(lead)) ;
        }

        public bool CheckOperationsCondition(LeadShortModel lead)
        {
            List<DateTime> dates = new List<DateTime>();
            PeriodModel period = new PeriodModel
            {
                To = DateTime.Now.ToString(),
                From = DateTime.Now.Subtract(Const.PERIOD_FOR_CHECK_TRANSACTIONS_FOR_VIP).ToString()
            };
            var transactions = _requests.GetTransactionsByPeriod(lead, period);
            foreach (var tr in transactions)
            {
                if (tr.TransactionType == TransactionType.Withdraw)
                {
                    transactions.Remove(tr);
                }
                if (tr.TransactionType == TransactionType.Transfer)
                {
                    if (dates.Contains(tr.Date))
                    {
                        transactions.Remove(tr);
                    }
                    else
                    {
                        dates.Add(tr.Date);
                    }
                }
            }

            return (transactions.Count >= Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP);
        }

        public bool CheckBalanceCondition(LeadShortModel lead)
        {
            decimal sumDeposit = 0;
            decimal sumWithdraw = 0;
            PeriodModel period = new PeriodModel {
                To = DateTime.Now.ToString(), 
                From = DateTime.Now.Subtract(Const.PERIOD_FOR_CHECK_SUM_FOR_VIP).ToString() };
            var transactions = _requests.GetTransactionsByPeriod(lead, period);
            foreach (var tr in transactions)
            {
                if (tr.TransactionType == TransactionType.Deposit)
                {
                    sumDeposit += tr.Amount;
                }
                if (tr.TransactionType == TransactionType.Withdraw)
                {
                    sumWithdraw += tr.Amount;
                }
            }
            if (Math.Abs(sumWithdraw) > sumDeposit + Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP) return true;
            return false;
        }

        public bool CheckBirthdayCondition(string bDay)
        {
            var _dateMonthAndDay = "MM.dd";
            //bDay = bDay.Substring(5, 5);
            DateTime birthDay = Convert.ToDateTime(bDay);
            var birth = birthDay.ToString(_dateMonthAndDay);
            var date = Convert.ToDateTime(birth);

            //var date = DateTime.ParseExact(bDay, "MM.dd", null);
            return (date <= DateTime.Now && date.AddDays(Const.COUNT_DAY_AFTER_BDAY_FOR_VIP) > DateTime.Now);
        }
    }
}
