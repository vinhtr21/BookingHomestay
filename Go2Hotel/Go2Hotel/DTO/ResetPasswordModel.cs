using System.ComponentModel.DataAnnotations;

namespace Go2Hotel.DTO
{
    public class ResetPasswordModel
    {
        public string? Email { get; set; }
        [Required(ErrorMessage = "NewPassword is required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "ConfirmPassword is required"), Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }

    }
}
