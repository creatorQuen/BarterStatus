using LeadStatusUpdater.Enums;
using System;
using System.Collections.Generic;

namespace LeadStatusUpdater.Models
{
    public class AccountBusinessModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public Currency Currency { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Closed { get; set; }
        public bool IsDeleted { get; set; }
        public List<TransactionBusinessModel> Transactions { get; set; }
        public List<TransferBusinessModel> Transfers { get; set; }
        public decimal Balance { get; set; }
    }
}
