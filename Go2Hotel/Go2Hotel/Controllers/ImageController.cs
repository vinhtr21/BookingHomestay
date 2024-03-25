using Go2Hotel.DTO;
using Go2Hotel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Imaging;

namespace Go2Hotel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly Booking_HomestayContext _context;
        private readonly IConfiguration _config;

        public ImageController(Booking_HomestayContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request, CancellationToken cancellationToken)
        {
            if(request.file == null || request.file.Length == 0)
            {
                return BadRequest("please select a valid image");
            }

            try
            {
                using(var ms = new MemoryStream())
                {
                    await request.file.CopyToAsync(ms);
                    var base64String = Convert.ToBase64String(ms.ToArray());
                    var img = new Image
                    {
                        HomestayId = request.HomestayId,
                        Image1 = base64String,
                    };
                    _context.Images.Add(img);
                    await _context.SaveChangesAsync(cancellationToken);
                    return Ok(new {img.ImageId, img.HomestayId, img.Image1});
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("list/{id}")]
        public async Task<ActionResult<Image>> GetAllImagesByHomestayId([FromRoute] long id)
        {
            if (_context.Images == null)
            {
                return NotFound();
            }
            var images = await _context.Images.Where(x => x.HomestayId == id).ToListAsync();

            if (images == null)
            {
                return NotFound();
            }

            return Ok(images);
        }

        [HttpGet("homestayId/{id}")]
        public async Task<ActionResult<Image>> GetImageByHomestayId([FromRoute] long id)
        {
            if (_context.Images == null)
            {
                return NotFound();
            }
            var images = await _context.Images.Where(x => x.HomestayId == id).FirstOrDefaultAsync();

            if (images == null)
            {
                return NotFound();
            }

            return Ok(images);
        }
        //[HttpPost("decodeToImage")]
        //public async Task<IActionResult> DecodeBase64ToImg([FromBody] string base64String)
        //{
        //    try
        //    {
        //        // Convert Base64 string to byte array
        //        byte[] imageBytes = Convert.FromBase64String(base64String);

        //        // Create a MemoryStream from the byte array
        //        using (MemoryStream ms = new MemoryStream(imageBytes))
        //        {
        //            // Create an Image object from the MemoryStream
        //            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

        //            // Return the image as a file
        //            using (MemoryStream outputStream = new MemoryStream())
        //            {
        //                image.Save(outputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        //                return File(outputStream.ToArray(), "image/jpeg");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any errors
        //        return BadRequest($"Error decoding image: {ex.Message}");
        //    }

        //}

    }
}
