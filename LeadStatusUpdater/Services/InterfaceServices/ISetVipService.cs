using LeadStatusUpdater.Models;

namespace LeadStatusUpdater.Services
{
    public interface ISetVipService
    {
        bool CheckBalanceCondition(LeadOutputModel lead);
        bool CheckBirthdayCondition(int birthDay, int birthMonth);
        bool CheckOneLead(LeadOutputModel lead);
        bool CheckOperationsCondition(LeadOutputModel lead);
        void Process();
    }
}
