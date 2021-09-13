using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;

namespace LeadStatusUpdater.Services
{
    public class SetVipService : ISetVipService
    {
        private IRequestsSender _requests;
        private readonly ILogger<Worker> _logger;
        private const string _dateFormat = "MM.dd";

        public SetVipService(ILogger<Worker> logger, IRequestsSender sender)
        {
            _requests = sender;
            _logger = logger;
        }

        public void Process()
        {
            var leads = _requests.GetAllLeads(); //get leads by filters (roles 2 3)


            leads.ForEach(lead => 
            {
                var newRole = CheckOneLead(lead) ? Role.Vip : Role.Regular;
                if (lead.Role != newRole)
                {

                    if (newRole == Role.Vip)
                    {
                        //_logger.LogInformation($"Vip status was given to Lead: Id[{lead.Id}], FirstName[{lead.FirstName}], LastName[{lead.LastName}], Patronymic[{lead.Patronymic}], " +
                        //    $"Email:[{lead.Email}]");

                        Log.Information($"Vip status was given to Lead: Id[{lead.Id}], FirstName[{lead.FirstName}], LastName[{lead.LastName}], Patronymic[{lead.Patronymic}], " +
                            $"Email:[{lead.Email}]");
                    }
                    else 
                    {
                        //_logger.LogInformation($"Vip status was taken from Lead: Id[{lead.Id}], FirstName[{lead.FirstName}], LastName[{lead.LastName}], Patronymic[{lead.Patronymic}], " +
                        //    $"Email:[{lead.Email}]");

                        Log.Information($"Vip status was taken from Lead: Id[{lead.Id}], FirstName[{lead.FirstName}], LastName[{lead.LastName}], Patronymic[{lead.Patronymic}], " +
                            $"Email:[{lead.Email}]");
                    }

                }
            });
            //foreach (var lead in leads)
            //{
            //    var newRole = CheckOneLead(lead) ? Role.Vip : Role.Regular;
            //    if(lead.Role != newRole)
            //    {
            //        _requests.ChangeStatus(lead.Id, newRole);
            //    }
            //}
        }

        public bool CheckOneLead(LeadOutputModel lead)
        {
            return (
                //CheckBirthdayCondition(lead.BirthDate) ||
                CheckOperationsCondition(lead) 
                //|| CheckBalanceCondition(lead)
                );
        }


        public bool CheckOperationsCondition(LeadOutputModel lead)
        {
            int transactionsCount = 0;
            foreach (var account in lead.Accounts)
            {
                TimeBasedAcquisitionInputModel period = new TimeBasedAcquisitionInputModel
                {
                    To = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                    From = DateTime.Now.AddDays(-Const.PERIOD_FOR_CHECK_TRANSACTIONS_FOR_VIP).ToString("dd.MM.yyyy HH:mm"),
                    AccountId = account.Id
                };

                var accountsWithTransactions = _requests.GetTransactionsByPeriod(period);

                if(accountsWithTransactions.FirstOrDefault().Transactions.Count > 0)
                {
                    transactionsCount += accountsWithTransactions.FirstOrDefault().Transactions.
                    Where(t => t.TransactionType == TransactionType.Deposit).Count();
                }
                if(accountsWithTransactions.FirstOrDefault().Transfers.Count > 0)
                {
                    transactionsCount += accountsWithTransactions.FirstOrDefault().Transfers.Count();
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

            return (Math.Abs(sumWithdraw) > sumDeposit + Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP);
        }

        public bool CheckBirthdayCondition(int birthDay, int birthMonth)
        {
            var birthDayAndMonth = new LeadBirthDateFilterModel { BirthDay = birthDay, BirthMonth = birthMonth };

            string bDay = "nulll";
            bDay = bDay.Substring(5, 5);
            var date = DateTime.ParseExact(bDay, "MM.dd", null);


            return (date <= DateTime.Now && date.AddDays(Const.COUNT_DAY_AFTER_BDAY_FOR_VIP) > DateTime.Now);
        }

    }
}
