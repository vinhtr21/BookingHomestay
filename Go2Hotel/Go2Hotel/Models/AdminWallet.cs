using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class AdminWallet
    {
        public int WalletId { get; set; }
        public int AdminId { get; set; }
        public decimal? WalletAmount { get; set; }
    }
}
