using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class Owner
    {
        public int OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerPassword { get; set; }
        public string? OwnerEmail { get; set; }
        public string? OwnerAvatar { get; set; }
        public string? OwnerAddress { get; set; }
        public string? OwnerAddress2 { get; set; }
        public string? OwnerCccd { get; set; }
        public string? OwnerPhone { get; set; }
        public string? OwnerCity { get; set; }
        public decimal? OwnerCredit { get; set; }
        public byte? OwnerStatus { get; set; }
        public string? ResetPasswordToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }
    }
}
