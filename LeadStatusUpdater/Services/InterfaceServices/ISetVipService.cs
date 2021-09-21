using LeadStatusUpdater.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Services
{
    public interface ISetVipService
    {
        Task<bool> CheckBalanceCondition(List<TransactionOutputModel> transactions);
        Task<bool> CheckBirthdayCondition(LeadOutputModel lead);
        Task<bool> CheckOneLead(LeadOutputModel lead);
        Task<bool> CheckOperationsCondition(List<TransactionOutputModel> transactions);
        void Process(object obj);
    }
}
