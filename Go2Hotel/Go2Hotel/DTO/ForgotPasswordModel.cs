using System.ComponentModel.DataAnnotations;

namespace Go2Hotel.DTO
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public DateTime? ExpireDate { get; set; }

    }
}
