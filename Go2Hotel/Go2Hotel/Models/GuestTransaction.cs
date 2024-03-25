using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class GuestTransaction
    {
        public int TransactionId { get; set; }
        public int WalletId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? TransactionAmount { get; set; }
        public decimal? TransactionRemain { get; set; }
        public bool? TransactionStatus { get; set; }
        public string? TransactionContent { get; set; }
    }
}
