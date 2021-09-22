using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeadStatusUpdaterTests.DataHelpers
{
    public static class SetVipServiceData
    {
        private const string _dateFormat = "dd.MM.yyyy";
        private const int _daysForSettingBirthDate = 30;
        private static List<string> _currencies = new List<string>
        {
            Currency.EUR.ToString(),
            Currency.JPY.ToString(),
            Currency.RUB.ToString(),
            Currency.USD.ToString()
        };
        private static List<int> _accounts = Enumerable.Range(1, _currencies.Count).ToList();

        public static IEnumerable GetDataForCheckLead()
        {
            yield return new object[] { GetLeadForBirhdayCheck(), null, null, true };
            yield return new object[] { GetLeadForOperationOrBalanceCheck(), _accounts,
                GetTransactionsForOperationCheck(), true};
            yield return new object[] { GetLeadForOperationOrBalanceCheck(), _accounts,
                GetTransactionsForBalanceCheck(), true};
            yield return new object[] { GetLeadForOperationOrBalanceCheck(), new List<int>{ 1}, new List<TransactionOutputModel>
                { GetTransactionByType(1, TransactionType.Transfer)}, false};
            yield return new object[] { GetLeadForOperationOrBalanceCheck(), null, null, false};

        }

        public static IEnumerable GetDataForBirthdayCheck()
        {
            yield return new object[] { GetLeadForBirhdayCheck(), true };
            yield return new object[] { GetLeadForOperationOrBalanceCheck(), false };
        }

        public static IEnumerable GetDataForOperationCheck()
        {
            yield return new object[] { GetTransactionsForOperationCheck(), true };
            yield return new object[] { new List<TransactionOutputModel>{GetTransactionByType(1, TransactionType.Deposit)}, false};
        }

        public static IEnumerable GetDataForBalanceCheck()
        {
            yield return new object[] { GetTransactionsForBalanceCheck(), true };
            yield return new object[] { new List<TransactionOutputModel>{GetTransactionByType(1, TransactionType.Deposit)}, false};
        }

        public static LeadOutputModel GetLeadForBirhdayCheck()
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
                    new AccountOutputModel{ Id = 1 }
                }
            };
        }

        public static LeadOutputModel GetLeadForOperationOrBalanceCheck()
        {
            return new LeadOutputModel
            {
                Id = 1,
                FirstName = "Name",
                LastName = "Last",
                Patronymic = "Patronymic",
                Email = "email@email.com",
                BirthDate = DateTime.Now.AddDays(-_daysForSettingBirthDate).ToString(_dateFormat),
                Role = Role.Regular,
                Accounts = new List<AccountOutputModel>{
                    new AccountOutputModel{ Id = 1},
                    new AccountOutputModel{ Id = 2},
                    new AccountOutputModel{ Id = 3},
                    new AccountOutputModel{ Id = 4},
                    new AccountOutputModel{ Id = 5},
                }
            };
        }

        public static List<TransactionOutputModel> GetTransactionsForOperationCheck()
        {
            var transactions = new List<TransactionOutputModel>();

            for (int i = 0; i < Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP/2; i++)
            {
                transactions.Add(GetTransactionByType(i, TransactionType.Transfer));

            }

            for (int i = Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP / 2; i < Const.COUNT_TRANSACTIONS_IN_PERIOD_FOR_VIP + 1; i++)
            {
                transactions.Add(GetTransactionByType(i, TransactionType.Deposit));

            }
            return transactions;
        } 

        public static List<TransactionOutputModel> GetTransactionsForBalanceCheck()
        {
            var transactions = new List<TransactionOutputModel>();
            decimal sum = 0;
            int transactionId = 1;
            while(sum < Const.SUM_DIFFERENCE_DEPOSIT_AND_WITHRAW_FOR_VIP)
            {
                var transactionType = transactionId % 2 == 0 ? TransactionType.Deposit : TransactionType.Withdraw;
                sum += transactionType == TransactionType.Withdraw ? -300m : 1000m;
                transactions.Add(GetTransactionByType(transactionId, transactionType));
                transactionId++;
            }

            return transactions;
        }      

        public static TransactionOutputModel GetTransactionByType(int transactionId, TransactionType transactionType)
        {
            Random random = new Random();
            var accountId = random.Next(1, _currencies.Count);
            var amount = transactionType == TransactionType.Withdraw ? -300m : 1000m; 
            return new TransactionOutputModel
            {
                Id = transactionId,
                TransactionType = transactionType,
                Date = DateTime.Now,
                Amount = amount,
                AccountId = accountId,
                Currency = Currency.RUB.ToString()
            };
        }

        public static List<LeadOutputModel> GetLeadsOutputModel()
        {
            return new List<LeadOutputModel>
            {
                new LeadOutputModel{
                    Id = 1,
                    FirstName = "Name1",
                    LastName = "Last1",
                    Patronymic = "Patronymic1",
                    Email = "email1@email.com",
                    BirthDate = DateTime.Now.ToString(_dateFormat),
                    Role = Role.Regular,
                    Accounts = new List<AccountOutputModel>{
                        new AccountOutputModel{ Id = 1 }
                    },
                },
                new LeadOutputModel{
                    Id = 2,
                    FirstName = "Name2",
                    LastName = "Last2",
                    Patronymic = "Patronymic2",
                    Email = "email2@email.com",
                    BirthDate = DateTime.Now.AddDays(-4).ToString(_dateFormat),
                    Role = Role.Regular,
                    Accounts = new List<AccountOutputModel>{
                        new AccountOutputModel{ Id = 2 }

                    }
                },
                new LeadOutputModel{
                    Id = 3,
                    FirstName = "Name3",
                    LastName = "Last3",
                    Patronymic = "Patronymic3",
                    Email = "email3@email.com",
                    BirthDate = DateTime.Now.AddDays(-10).ToString(_dateFormat),
                    Role = Role.Vip,
                    Accounts = new List<AccountOutputModel>{
                        new AccountOutputModel{ Id = 3 }
                    }
                },
                new LeadOutputModel{
                    Id = 4,
                    FirstName = "Name4",
                    LastName = "Last4",
                    Patronymic = "Patronymic4",
                    Email = "email4@email.com",
                    BirthDate = DateTime.Now.AddDays(-_daysForSettingBirthDate).ToString(_dateFormat),
                    Role = Role.Regular,
                    Accounts = new List<AccountOutputModel>{
                        new AccountOutputModel{ Id = 4 }
                    }
                },
                new LeadOutputModel{
                    Id = 5,
                    FirstName = "Name5",
                    LastName = "Last5",
                    Patronymic = "Patronymic5",
                    Email = "email5@email.com",
                    BirthDate = DateTime.Now.AddDays(-_daysForSettingBirthDate).ToString(_dateFormat),
                    Role = Role.Regular,
                    Accounts = new List<AccountOutputModel>{
                        new AccountOutputModel{ Id = 5 }
                    }
                },
                new LeadOutputModel{
                    Id = 6,
                    FirstName = "Name6",
                    LastName = "Last6",
                    Patronymic = "Patronymic6",
                    Email = "email6@email.com",
                    BirthDate = DateTime.Now.AddDays(-_daysForSettingBirthDate).ToString(_dateFormat),
                    Role = Role.Vip,
                    Accounts = new List<AccountOutputModel>{
                        new AccountOutputModel{ Id = 6 }
                    }
                }
            };
        }

        public static List<LeadIdAndRoleInputModel> GetLeadsToChangeStatusList()
        {
            return new List<LeadIdAndRoleInputModel>
            {
                new LeadIdAndRoleInputModel{ Id = 1, Role = Role.Vip},
                new LeadIdAndRoleInputModel{ Id = 2, Role = Role.Vip},
                new LeadIdAndRoleInputModel{ Id = 4, Role = Role.Vip},
                new LeadIdAndRoleInputModel{ Id = 5, Role = Role.Vip},
                new LeadIdAndRoleInputModel{ Id = 6, Role = Role.Regular},
            };

        }

        public static List<TransactionOutputModel> GetTransactionsByLeadId(int leadId)
        {
            var transactions = new List<TransactionOutputModel>();
            switch (leadId)
            {
                case 4:
                    transactions = GetTransactionsForOperationCheck();
                    break;
                case 5:
                    transactions = GetTransactionsForBalanceCheck();
                    break;
                case 6:
                    transactions = new List<TransactionOutputModel> 
                    { 
                        new TransactionOutputModel
                        {
                            Id = 1,
                            TransactionType = TransactionType.Deposit,
                            Date = DateTime.Now,
                            Amount = 1000m,
                            AccountId = 6,
                            Currency = Currency.RUB.ToString()
                        }
                    };
                    break;
            }
            return transactions;
        }
    }
}

