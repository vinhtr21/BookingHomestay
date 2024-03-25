using System.ComponentModel.DataAnnotations;

namespace Go2Hotel.DTO
{
    public class UserDTO
    {
        public int? GuestId { get; set; }

        public string? GuestName { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string GuestPhone { get; set; }

        public string? GuestEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string GuestPassword { get; set; }

        public string? GuestAvatar { get; set; }

        public string? GuestAddress { get; set; }

        public string? GuestCccd { get; set; }

        public byte? GuestStatus { get; set; }

    }
}
