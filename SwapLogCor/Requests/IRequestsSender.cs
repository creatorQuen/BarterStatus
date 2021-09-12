using SwapLogCor.Models;
using System.Collections.Generic;

namespace SwapLogCor
{
    public interface IRequestsSender
    {
        List<LeadShortModel> GetAllLeads();
        List<TransactionBusinessModel> GetTransactionsByPeriod(LeadShortModel lead, PeriodModel period);
        LeadOutputModel ChangeStatus(int leadId, bool status);
    }
}