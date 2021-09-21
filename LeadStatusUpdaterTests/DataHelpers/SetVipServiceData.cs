using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace LeadStatusUpdaterTests.DataHelpers
{
    public class SetVipServiceData
    {
        private static string _dateFormat = "dd.mm.yyyy";

        public static LeadOutputModel GetLead()
        {
            return new LeadOutputModel
            {
                Id = 1,
                FirstName = "Name",
                LastName = "Last",
                Patronymic = "Patronymic",
                Email = "email@email.com",
                BirthDate = DateTime.Now.ToString(_dateFormat),
                Role = Role.Regular,
                Accounts = new List<AccountOutputModel>{
                    new AccountOutputModel
                    {
                        Id = 1, 
                        Currency = Currency.EUR,
                        CreatedOn = "01.01.2020",
                        Balance = 1000m,
                        Transactions = GetTransactions(1)
                    }
                }
            };
        }

        static List<TransactionBusinessModel> GetTransactions(int accountId)
        {
            switch (accountId)
            {
                case 1:
                    return new List
                    break;
            }
        }

    }
}
