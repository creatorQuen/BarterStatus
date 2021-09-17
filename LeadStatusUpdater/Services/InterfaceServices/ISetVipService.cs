using LeadStatusUpdater.Models;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Services
{
    public interface ISetVipService
    {
        bool CheckBalanceCondition(LeadOutputModel lead);
        Task<bool> CheckBirthdayCondition(LeadOutputModel lead);
        Task <bool> CheckOneLead(LeadOutputModel lead);
        bool CheckOperationsCondition(LeadOutputModel lead);
        Task Process();
    }
}
