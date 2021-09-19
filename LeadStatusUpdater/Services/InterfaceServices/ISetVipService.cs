using LeadStatusUpdater.Models;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Services
{
    public interface ISetVipService
    {
        Task<bool> CheckBalanceCondition(LeadOutputModel lead);
        bool CheckBirthdayCondition(LeadOutputModel lead);
        Task<bool> CheckOneLead(LeadOutputModel lead);
        Task<bool> CheckOperationsCondition(LeadOutputModel lead);
        Task Process(object obj);
    }
}
