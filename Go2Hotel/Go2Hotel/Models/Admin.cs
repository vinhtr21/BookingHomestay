using System;
using System.Collections.Generic;

namespace Go2Hotel.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public string? AdminPassword { get; set; }
        public string? AdminEmail { get; set; }
        public string? ResetPasswordToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }
    }
}
