using BarterStatus.Models;

namespace BarterStatus.Services
{
    public interface ISetVipService
    {
        bool CheckBalanceCondition(LeadShortModel lead);
        bool CheckBirthdayCondition(string bDay);
        bool CheckOneLead(LeadShortModel lead);
        bool CheckOperationsCondition(LeadShortModel lead);
        void Process();
    }
}