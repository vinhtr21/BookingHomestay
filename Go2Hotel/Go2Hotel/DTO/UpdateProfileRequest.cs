namespace Go2Hotel.DTO
{
    public class UpdateProfileRequest
    {
        public int GuestId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestPhone { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestCccd { get; set; }
        public string? GuestAddress { get; set; }

    }
}
