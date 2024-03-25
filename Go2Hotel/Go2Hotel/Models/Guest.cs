using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class Guest
    {
        public int GuestId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestPhone { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPassword { get; set; }
        public string? GuestAvatar { get; set; }
        public string? GuestAddress { get; set; }
        public string? GuestCccd { get; set; }
        public byte? GuestStatus { get; set; }
        public string? ResetPasswordToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }
    }
}
