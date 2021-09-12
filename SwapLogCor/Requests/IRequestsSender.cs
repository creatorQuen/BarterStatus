using SwapLogCor.Enums;
using SwapLogCor.Models;
using System.Collections.Generic;

namespace SwapLogCor
{
    public interface IRequestsSender
    {
        List<LeadShortModel> GetAllLeads();
        LeadOutputModel ChangeStatus(int leadId, Role status);
        List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model);
    }
}