using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using System.Collections.Generic;

namespace LeadStatusUpdater
{
    public interface IRequestsSender
    {
        List<LeadOutputModel> GetAllLeads();
        LeadOutputModel ChangeStatus(int leadId, Role status);
        List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model);
    }
}