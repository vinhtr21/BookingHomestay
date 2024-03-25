using System.ComponentModel.DataAnnotations;

namespace Go2Hotel.DTO
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string GuestName { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string GuestPhone { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string GuestEmail { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string GuestPassword { get; set; }
        [Required(ErrorMessage = "ConfirmPassword is required"), Compare("GuestPassword")]
        public string ConfirmPassword { get; set; }
        public byte? GuestStatus { get; set; }
    }
}
