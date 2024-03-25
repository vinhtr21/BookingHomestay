using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class Rate
    {
        public int RateId { get; set; }
        public int GuestId { get; set; }
        public int HomestayId { get; set; }
        public byte? RateAmount { get; set; }
    }
}
