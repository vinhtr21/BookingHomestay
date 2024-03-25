using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class OwnerWallet
    {
        public int WalletId { get; set; }
        public int OwnerId { get; set; }
        public decimal? WalletAmount { get; set; }
    }
}
