using LeadStatusUpdater.Enums;
using System;

namespace LeadStatusUpdater.Models
{
    public class TransactionOutputModel
    {
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int AccountId { get; set; }
        public string Currency { get; set; }
    }
}
