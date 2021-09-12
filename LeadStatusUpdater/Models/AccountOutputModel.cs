using System.Collections.Generic;
using LeadStatusUpdater.Enums;

namespace LeadStatusUpdater.Models
{
    public class AccountOutputModel
    {
        public int Id { get; set; }
        public Currency Currency { get; set; }
        public string CreatedOn { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionBusinessModel> Transactions { get; set; }
    }
}