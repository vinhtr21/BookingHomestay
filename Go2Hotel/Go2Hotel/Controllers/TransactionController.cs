using Microsoft.AspNetCore.Mvc;
using Go2Hotel.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Go2Hotel.Payments;
using Go2Hotel.Payment;
using System.Net;

namespace Go2Hotel.Controllers
{
    [EnableCors("AllowAllOrigins")]
    [Route("[controller]")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly Booking_HomestayContext _context;
        public TransactionController(Booking_HomestayContext context)
        {
            _context = context;
        }

        #region Thanh toán vnpay
        private string UrlPayment(string orderCode)
        {
            var urlPayment = "";
            var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == int.Parse(orderCode));
            //Get Config Info
            string vnp_Returnurl = "http://127.0.0.1:5500/UserPage/success-payment.html"; //URL nhan ket qua tra ve 
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = "ADH2MKPG"; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = "XIEWSZDVZMTOMCLXMYXLFUFEAKPFQZKP"; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)(booking.TotalCost * 100);
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_BankCode", "VNBANK");

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Helper.GetIpAddress(HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Đặt phòng homestay :" + booking.BookingId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            Random rd = new Random();
            var refCode = "G2H" + orderCode;
            vnpay.AddRequestData("vnp_TxnRef", refCode); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion

        [HttpPost("Pay")]
        public IActionResult CheckBooking([FromBody] BookingForm bookingForm)
        {
            try
            {
                var code = new { Success = false, Url = "" };
                if (bookingForm != null)
                {
                    var homstay = _context.Homestays.FirstOrDefault(x => x.HomestayId == bookingForm.HomstayId);
                    decimal? price = homstay.HomestayPrice;

                    TimeSpan duration = (TimeSpan)(bookingForm.BookingTo - bookingForm.BookingFrom);
                    int count = duration.Days;

                    Booking booking = new Booking();
                    booking.GuestId = bookingForm.GuestId;
                    booking.HomstayId = bookingForm.HomstayId;
                    booking.CheckinTime = bookingForm.CheckinTime;
                    booking.CheckoutTime = bookingForm.CheckoutTime;
                    booking.BookingFrom = bookingForm.BookingFrom;
                    booking.BookingTo = bookingForm.BookingTo;
                    booking.TotalCost = price * count;
                    booking.BookingDate = DateTime.Now;
                    booking.BookingPhone = bookingForm.BookingPhone;

                    var check = _context.Bookings.Where(x => x.HomstayId == bookingForm.HomstayId).ToList();
                    if (!checkBookingDate(check, (DateTime)booking.BookingFrom, (DateTime)booking.BookingTo))
                        return BadRequest();

                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    var url = UrlPayment("" + booking.BookingId);
                    code = new { Success = true, Url = url };

                    return Json(code);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {

                return BadRequest();
            }
        }

        private bool checkBookingDate(List<Booking> bookings, DateTime from, DateTime to)
        {
            foreach (Booking booking in bookings)
            {
                if (booking.BookingFrom <= from && booking.BookingTo >= to) return false;
                else if (booking.BookingFrom >= from && booking.BookingTo <= to) return false;
                else if (booking.BookingFrom <= from && booking.BookingTo <= to) return false;
                else if
                    (booking.BookingFrom >= from && booking.BookingTo >= to) return false;
            }
            return true;
        }

        [HttpGet("wallet/{id}")]
        public IActionResult WalletAmmount(int id)
        {
            GuestWallet guestWallet = _context.GuestWallets.FirstOrDefault(x => x.GuestId == id);
            if (guestWallet != null)
            {
                decimal money = guestWallet.WalletAmount.Value;
                return Ok(new { amount = money });
            }
            else return Ok(new { amount = 0 });
        }

        [HttpGet("GetDashboardInfo")]
        public async Task<IActionResult> GetDashboardInformation()
        {
            if (_context.GuestTransactions == null)

            {
                return NotFound();
            }

            var nowDate = DateTime.Now;
            var todayMoney = await _context.GuestTransactions.FirstOrDefaultAsync(x => x.TransactionDate.Value.Date == nowDate.Date);
            var previousMoney = await _context.GuestTransactions.FirstOrDefaultAsync(x => x.TransactionDate.Value.Date == nowDate.Date.AddDays(-1));
            var moPercent = (previousMoney?.TransactionAmount * 100) / todayMoney?.TransactionAmount;

            var todayBooking = await _context.GuestTransactions.Where(x => x.TransactionDate.Value.Date == nowDate.Date).ToListAsync();
            var previousBooking = await _context.GuestTransactions.Where(x => x.TransactionDate.Value.Date == nowDate.Date.AddDays(-1)).ToListAsync();
            var booPercent = (previousBooking?.Count * 100) / todayBooking?.Count;


            var totalClient = await _context.Guests.CountAsync();
            var totalBooking = await _context.Bookings.CountAsync();


            var homestay = await _context.GuestTransactions
                .Select(x => new
                {
                    TodayMon = todayMoney.TransactionAmount,
                    TodayBook = todayBooking.Count,
                    TotalCli = totalClient,
                    TotalBooking = totalBooking,
                    MoneyPercent = moPercent,
                    BookingPercent = booPercent,

                }).FirstOrDefaultAsync();

            if (homestay == null)
            {
                return NotFound();
            }

            return Ok(homestay);
        }
        [HttpGet("getTransactions")]
        public async Task<ActionResult<List<OwnerTransaction>>> GetOwnersTransactions()
        {
            if (_context.OwnerTransactions == null)
            {
                return NotFound();
            }
            var transactions = await _context.OwnerTransactions.ToListAsync();

            if (transactions == null)
            {
                return NotFound();
            }
            return transactions;
        }
    }

}


