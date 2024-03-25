using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int HomestayId { get; set; }
        public int GuestId { get; set; }
        public string? CommentContext { get; set; }
        public DateTime? CommentDate { get; set; }
    }
}
