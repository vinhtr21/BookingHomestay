using Go2Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Go2Hotel.Utility;
using Go2Hotel.DTO;

namespace Go2Hotel.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : Controller
    {
        private readonly Booking_HomestayContext _context;
        private EmailSender sender = new EmailSender();
        public GuestsController(Booking_HomestayContext context)
        {
            _context = context;
        }

        [HttpGet("listAll")]
        public async Task<ActionResult<List<Guest>>> GetAllGuest()
        {
            if (_context.Guests == null)
            {
                return NotFound();
            }
            var guest = await _context.Guests.ToListAsync();

            if (guest == null)
            {
                return NotFound();
            }

            return guest;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuestById([FromRoute] long id)
        {
            if (_context.Guests == null)
            {
                return NotFound();
            }
            var guest = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);

            if (guest == null)
            {
                return NotFound();
            }

            return guest;
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([Required] ForgotPasswordModel model)
        {
            var user = await _context.Guests.FirstOrDefaultAsync(u => u.GuestEmail == model.Email);
            if (user == null)
            {
                return BadRequest("email not found");
            }

            Random random = new Random();
            int randomNumber = random.Next(100000, 999999);
            string token = randomNumber.ToString();
            var expireTime = DateTime.UtcNow.AddMinutes(5);
            var forgotToken = new ForgotPasswordModel
            {
                Email = model.Email,
                ExpireDate = expireTime,
            };
            sender.SendMail(model.Email, "RESET PASSWORD!", $"The code is: {token} \n Code is available in 5 minutes since this email is sent \n DO NOT SHARE TO ANYONE FOR PRIVACY!!");
            try
            {
                user.ResetPasswordToken = token.ToString();
                await _context.SaveChangesAsync();
                return Ok(new { token = token });

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model, CancellationToken cancellation = default)
        {
            var user = await _context.Guests.FirstOrDefaultAsync(u => u.ResetPasswordToken == model.Token, cancellation);
            if (user == null)
            {
                return BadRequest();
            }
            try
            {
                user.GuestPassword = PasswordHasher.HashPassword(model.ConfirmPassword);
                user.ResetPasswordToken = null;
                await _context.SaveChangesAsync(cancellation);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //kien
        [HttpGet("search")]
        public IActionResult SearchProducts(string country, string city, string region, string street)
        {
            var query = _context.Homestays
                .AsQueryable();

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(p => p.HomestayCity.Contains(country));
            }
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.HomestayCity.Contains(city));
            }
            if (!string.IsNullOrEmpty(region))
            {
                query = query.Where(p => p.HomestayRegion.Contains(region));
            }
            if (!string.IsNullOrEmpty(street))
            {
                query = query.Where(p => p.HomestayStreet.Contains(street));
            }

            var result = query.ToList();
            return Ok(result);
        }

        private bool CheckGuestExists(int id)
        {
            return (_context.Guests?.Any(e => e.GuestId == id)).GetValueOrDefault();
        }


        [HttpPut("status/{id}")]
        public async Task<IActionResult> DisableGuestAccount([FromRoute] int id)
        {
            if (_context.Guests == null)
            {
                return NotFound();
            }
            var home = await _context.Guests.FindAsync(id);

            if (home == null)
            {
                return NotFound();
            }

            if (home.GuestStatus == 1)
            {
                home.GuestStatus = 0;
            }
            else
            {
                home.GuestStatus = 1;
            }
            _context.Entry(home).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateGuest([FromBody] UpdateProfileRequest request)
        {

            if (_context.Guests == null)
            {
                return NotFound();
            }
            var guests = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == request.GuestId);
            guests.GuestPhone = request.GuestPhone;
            guests.GuestName = request.GuestName;
            guests.GuestEmail = request.GuestEmail;
            guests.GuestCccd = request.GuestCccd;
            guests.GuestAddress = request.GuestAddress;
            try
            {
                _context.SaveChangesAsync();
                return Ok("update ok");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

    }
}
