using System.ComponentModel.DataAnnotations;

namespace Go2Hotel.DTO
{
    public class OwnerDTO
    {
        public int OwnerId { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? OwnerPassword { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string? OwnerPhone { get; set; }

    }
}
