using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using System.Collections.Generic;

namespace LeadStatusUpdater
{
    public interface IRequestsSender
    {
        List<LeadOutputModel> GetRegularAndVipLeads(int cursor);
        int ChangeStatus(List<LeadIdAndRoleInputModel> model);
        List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model);
        string GetAdminToken();
    }
}