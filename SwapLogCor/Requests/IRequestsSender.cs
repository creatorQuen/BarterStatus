using SwapLogCor.Models;
using System.Collections.Generic;

namespace SwapLogCor
{
    public interface IRequestsSender
    {
        List<LeadShortModel> GetAllLeads();
        LeadOutputModel ChangeStatus(int leadId, bool status);
        List<TransactionBusinessModel> GetTransactionsByPeriod(LeadShortModel lead, TimeBasedAcquisitionInputModel period);
    }
}