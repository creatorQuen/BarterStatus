using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using System.Collections.Generic;

namespace LeadStatusUpdater
{
    public interface IRequestsSender
    {
        List<LeadShortModel> GetAllLeads();
        LeadOutputModel ChangeStatus(int leadId, Role status);
        List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model);
    }
}