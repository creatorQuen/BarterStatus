using LeadStatusUpdater.Models;

namespace LeadStatusUpdater.Services
{
    public interface ISetVipService
    {
        bool CheckBalanceCondition(LeadOutputModel lead);
        bool CheckBirthdayCondition(LeadOutputModel lead);
        bool CheckOneLead(LeadOutputModel lead);
        bool CheckOperationsCondition(LeadOutputModel lead);
        void Process();
    }
}
