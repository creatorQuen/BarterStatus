using Microsoft.Extensions.Options;
using RestSharp;
using SwapLogCor.Models;
using SwapLogCor.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapLogCor.Services
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
            //Console.WriteLine("yesssssssss");
            //get all leads
            //for each lead get all transactions by period
            //delete withdraws
            //check conditions
            //
        }

        public bool CheckOneLead(LeadShortModel lead)
        {
            if (CheckOperationsCondition(lead) ||
            CheckBalanceCondition(lead) ||
            CheckBirthdayCondition(lead.BirthDate)) return true;
            return false;
        }

        public bool CheckOperationsCondition(LeadShortModel lead)
        {
            List<DateTime> dates = new List<DateTime>();
            var transactions = _requests.GetTransactionsByPeriod(lead.Id, Constants.PERIOD_FOR_CHECK_TRANSACTIONS_FOR_VIP);
            foreach (var tr in transactions)
            {
                if ((int)tr.TransactionType == 2)
                {
                    transactions.Remove(tr);
                }
                if ((int)tr.TransactionType == 3)
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

            if (transactions.Count >= Constants.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP) return true;
            return false;
        }

        public bool CheckBalanceCondition(LeadShortModel lead)
        {
            decimal sumDeposit = 0;
            decimal sumWithdraw = 0;
            var transactions = _requests.GetTransactionsByPeriod(lead.Id, 30);
            foreach (var tr in transactions)
            {
                if ((int)tr.TransactionType == 1)
                {
                    sumDeposit += tr.Amount;
                }
                if ((int)tr.TransactionType == 2)
                {
                    sumWithdraw += tr.Amount;
                }
            }
            if (Math.Abs(sumWithdraw) > sumDeposit + Constants.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP) return true;
            return false;
        }

        public bool CheckBirthdayCondition(string bDay)
        {
            bDay = bDay.Substring(5, 5);
            var date = DateTime.ParseExact(bDay, "MM.dd", null);
            if (date <= DateTime.Now && date.AddDays(Constants.COUNT_DAY_AFTER_BDAY_FOR_VIP) > DateTime.Now) return true;
            return false;
        }
    }
}
