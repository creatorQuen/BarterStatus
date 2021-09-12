using System;
using SwapLogCor.Enums;

namespace SwapLogCor.Models
{
    public class TransactionBusinessModel
    {
        public long Id { get; set; }
        public int AccountId { get; set; }
        public Currency Currency { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
