using Go2Hotel.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Go2Hotel.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly Booking_HomestayContext _context;

        public BookingController(Booking_HomestayContext context)
        {
            _context = context;
        }

        [HttpPost("delete/{id}/{guestId}")]
        public IActionResult DeleteBookingHomestay(int id, int guestId)
        {
            try
            {
                Booking booking = _context.Bookings.FirstOrDefault(x => x.BookingId.Equals(id));

                if (booking != null)
                {
                    var gWallet = _context.GuestWallets
                            .FirstOrDefault(x => x.GuestId == booking.GuestId);
                    if (gWallet == null)
                    {
                        GuestWallet wallet = new GuestWallet();
                        wallet.GuestId = booking.GuestId;
                        wallet.WalletAmount = 0;
                        _context.GuestWallets.Add(wallet);
                        _context.SaveChanges();

                        var gTransaction = new GuestTransaction();
                        gTransaction.WalletId = wallet.WalletId;
                        gTransaction.TransactionDate = DateTime.Now;
                        gTransaction.TransactionAmount = booking.TotalCost;
                        gTransaction.TransactionStatus = true;
                        gTransaction.TransactionRemain = 0;
                        gTransaction.TransactionContent = "Đã được hoàn tiền thành công số tiền : " + booking.TotalCost;
                        _context.GuestTransactions.Add(gTransaction);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var gTransaction = new GuestTransaction();
                        gTransaction.WalletId = gWallet.WalletId;
                        gTransaction.TransactionDate = DateTime.Now;
                        gTransaction.TransactionAmount = booking.TotalCost;
                        gTransaction.TransactionStatus = true;
                        gTransaction.TransactionRemain = 0;
                        gTransaction.TransactionContent = "Đã được hoàn tiền thành công số tiền : " + booking.TotalCost;
                        _context.GuestTransactions.Add(gTransaction);
                        _context.SaveChanges();
                    }

                    GuestWallet guestWallet = _context.GuestWallets.FirstOrDefault(x => x.GuestId.Equals(guestId));
                    guestWallet.WalletAmount += booking.TotalCost;
                    _context.GuestWallets.Update(guestWallet);
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                    return Ok();
                }
                else return BadRequest();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("booking_history")]
        public async Task<ActionResult> GetBookingHistory(int userId)
        {
            var bookings = _context.Bookings.Where(x => x.GuestId == userId).ToList();
            if (bookings.Count == 0)
            {
                return Ok("u have not booked any homestay");
            }
            return Ok(bookings);
        }
    }
}
