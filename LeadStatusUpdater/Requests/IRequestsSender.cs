using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using System.Collections.Generic;

namespace LeadStatusUpdater
{
    public interface IRequestsSender
    {
        List<LeadOutputModel> GetRegularAndVipLeads(string adminToken, int cursor);
        int ChangeStatus(List<LeadIdAndRoleInputModel> model, string adminToken);
        List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model, string adminToken);
        string GetAdminToken();
    }
}