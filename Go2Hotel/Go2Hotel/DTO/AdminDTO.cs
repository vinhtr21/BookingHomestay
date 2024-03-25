using System.ComponentModel.DataAnnotations;

namespace Go2Hotel.DTO
{
    public class AdminDTO
    {
        public int AdminId { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? AdminEmail { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? AdminPassword { get; set; }

    }
}
