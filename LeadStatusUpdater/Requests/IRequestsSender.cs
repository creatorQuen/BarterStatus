using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using System.Collections.Generic;

namespace LeadStatusUpdater
{
    public interface IRequestsSender
    {
        List<LeadOutputModel> GetRegularAndVipLeads(string adminToken, int cursor);
        LeadOutputModel ChangeStatus(int leadId, Role status, string adminToken);
        List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model, string adminToken);
        string GetAdminToken();
    }
}