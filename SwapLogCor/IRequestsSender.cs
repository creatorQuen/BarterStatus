using SwapLogCor.Models;
using System.Collections.Generic;

namespace SwapLogCor
{
    public interface IRequestsSender
    {
        void DeleteWithdrawTransactionsFromAllTransactions(List<TransactionBusinessModel> transactions);
        List<LeadShortModel> GetAllLeads();
        List<TransactionBusinessModel> GetTransactionsByPeriod(int id, int period);
        List<TransactionBusinessModel> GetTransactionsWithoutWithdrawsByLead(LeadShortModel lead);
    }
}