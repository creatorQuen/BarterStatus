using LeadStatusUpdater.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Services
{
    public interface ISetVipService
    {
        Task<bool> CheckBalanceCondition(List<TransactionOutputModel> transactions);
        bool CheckBirthdayCondition(LeadOutputModel lead);
        Task<bool> CheckOneLead(LeadOutputModel lead);
        void Process(object obj);
    }
}
