using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public int GuestId { get; set; }
        public int HomstayId { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public DateTime? BookingFrom { get; set; }
        public DateTime? BookingTo { get; set; }
        public decimal? TotalCost { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? BookingPhone { get; set; }
    }
}
