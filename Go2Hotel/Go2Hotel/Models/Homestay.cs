using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class Homestay
    {
        public int HomestayId { get; set; }
        public string? HomestayName { get; set; }
        public int OwnerId { get; set; }
        public byte? HomestaySodo { get; set; }
        public decimal? HomestayPrice { get; set; }
        public string? HomestayCountry { get; set; }
        public string? HomestayCity { get; set; }
        public string? HomestayRegion { get; set; }
        public string? HomestayStreet { get; set; }
        public bool? HomestayStatus { get; set; }
        public int HomestayType { get; set; }
        public string? HomestayDescription { get; set; }
        public byte HomestayBedroom { get; set; }

        public virtual IEnumerable<Image>? Images { get; set; }
    }
}
