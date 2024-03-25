using Go2Hotel.Models;
using Go2Hotel.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Go2Hotel.DTO;

namespace Go2Hotel.Controllers
{
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly Booking_HomestayContext _context;
        private readonly IConfiguration _config;

        public RegisterController(Booking_HomestayContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Guest>> Register([FromBody] RegisterRequest guest)
        {
            if (CheckGuestExist(guest.GuestPhone) == true)
            {
                return BadRequest(guest.GuestPhone + " has been registered. try another phone number instead");
            }



            guest.GuestPassword = PasswordHasher.HashPassword(guest.GuestPassword);
            guest.GuestStatus = 1;
            Guest g = new Guest()
            {
                GuestPhone = guest.GuestPhone,
                GuestPassword = guest.GuestPassword,
                GuestStatus = guest.GuestStatus,
                GuestName = guest.GuestName,
                GuestEmail = guest.GuestEmail
            };
            _context.Guests.Add(g);
            await _context.SaveChangesAsync();
            return Ok("register successfully");
        }


        private bool CheckGuestExist(string phone)
        {
            Guest g = _context.Guests.FirstOrDefault(g => g.GuestPhone == phone);
            if (g == null)
            {
                return false;
            }
            return true;
        }
    }
}
