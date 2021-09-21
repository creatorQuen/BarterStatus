using LeadStatusUpdater.Models;
using System.Collections.Generic;

namespace LeadStatusUpdater
{
    public interface IRequestsSender
    {
        List<LeadOutputModel> GetRegularAndVipLeads(int cursor);
        int ChangeStatus(List<LeadIdAndRoleInputModel> model);
        List<TransactionOutputModel> GetTransactionsByPeriod(List<int> accountIds);
        string GetAdminToken();
    }
}