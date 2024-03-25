namespace Go2Hotel.DTO
{
    public class UploadImageRequest
    {
        public int HomestayId { get; set; }
        public IFormFile file { get; set; }
    }
}
