using BarterStatus.Models;
using System.Collections.Generic;

namespace BarterStatus
{
    public interface IRequestsSender
    {
        List<LeadShortModel> GetAllLeads();
        List<TransactionBusinessModel> GetTransactionsByPeriod(LeadShortModel lead, PeriodModel period);
        public void SetVipStatus(int leadId, bool status);
    }
}