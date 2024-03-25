using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class GuestWallet
    {
        public int WalletId { get; set; }
        public int GuestId { get; set; }
        public decimal? WalletAmount { get; set; }
    }
}
