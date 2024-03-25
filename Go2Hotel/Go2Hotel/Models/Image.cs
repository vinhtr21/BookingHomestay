using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class Image
    {
        public int ImageId { get; set; }
        public int HomestayId { get; set; }
        public string? Image1 { get; set; }
    }
}
