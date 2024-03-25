using Go2Hotel.Models;
using Go2Hotel.Payments;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace Go2Hotel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class vnpay_return : Controller
    {
        private readonly Booking_HomestayContext _context;
        public vnpay_return(Booking_HomestayContext context)
        {
            _context = context;
        }

        private string ExtractFront(string input)
        {
            int index = input.IndexOf('=');
            if (index != -1)
            {
                return input.Substring(0, index);
            }
            else
            {
                // Handle case when '=' is not found
                return input;
            }
        }
        private string ExtractBehind(string input)
        {
            int index = input.IndexOf('=');
            if (index != -1 && index < input.Length - 1)
            {
                return input.Substring(index + 1);
            }
            else
            {
                // Handle case when '=' is not found or it's at the end of the string
                return string.Empty;
            }
        }

        [HttpGet("/vnpay_return/{TxnRef}/{SecureHash}/{ResponseCode}/{TransactionStatus}")]
        public IActionResult GetString(string TxnRef, string SecureHash, string ResponseCode, string TransactionStatus)
        {
            string vnp_HashSecret = "XIEWSZDVZMTOMCLXMYXLFUFEAKPFQZKP"; //Chuoi bi mat
            VnPayLibrary vnpay = new VnPayLibrary();
            
            string orderCode = TxnRef;
            //long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = ResponseCode;
            string vnp_TransactionStatus = TransactionStatus;
            String vnp_SecureHash = SecureHash;
            //String TerminalID = vnpay.GetResponseData("vnp_TmnCode");
            //long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            //String bankCode = vnpay.GetResponseData("vnp_BankCode");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            if (checkSignature)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    string pattern = @"G2H(\d+)";
                    Match match = Regex.Match(orderCode, pattern);
                    var booking = _context.Bookings.FirstOrDefault(x => x.BookingId == int.Parse(match.Groups[1].Value));
                    if (booking != null)
                    {
                        var gWallet = _context.GuestWallets
                            .FirstOrDefault(x => x.GuestId == booking.GuestId);
                        if (gWallet == null)
                        {
                            GuestWallet guestWallet = new GuestWallet();
                            guestWallet.GuestId = booking.GuestId;
                            guestWallet.WalletAmount = 0;
                            _context.GuestWallets.Add(guestWallet);
                            _context.SaveChanges();

                            var gTransaction = new GuestTransaction();
                            gTransaction.WalletId = guestWallet.WalletId;
                            gTransaction.TransactionDate = DateTime.Now;
                            gTransaction.TransactionAmount = booking.TotalCost;
                            gTransaction.TransactionStatus = true;
                            gTransaction.TransactionRemain = 0;
                            gTransaction.TransactionContent = "Đã đặt phòng thành công! Mã thanh toán: " + orderCode;
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
                            gTransaction.TransactionContent = "Đã đặt phòng thành công! Mã thanh toán: " + orderCode;
                            _context.GuestTransactions.Add(gTransaction);
                            _context.SaveChanges();
                        }

                        var homeStay = _context.Homestays
                            .FirstOrDefault(x => x.HomestayId == booking.HomstayId);
                        var oWallet = _context.OwnerWallets.FirstOrDefault(x => x.OwnerId == homeStay.OwnerId);
                        if (oWallet == null)
                        {
                            OwnerWallet ownerWallet = new OwnerWallet();
                            ownerWallet.OwnerId = homeStay.OwnerId;
                            ownerWallet.WalletAmount = 0;
                            _context.OwnerWallets.Add(ownerWallet);
                            _context.SaveChanges();

                            var oTransaction = new OwnerTransaction();
                            oTransaction.WalletId = ownerWallet.WalletId;
                            oTransaction.TransactionDate = DateTime.Now;
                            oTransaction.TransactionAmount = booking.TotalCost;
                            oTransaction.TransactionStatus = true;
                            oTransaction.TransactionRemain = 0;
                            oTransaction.TransactionContent = "Có Phòng đã được đặt thành công! Mã thanh toán: " + orderCode;
                            _context.OwnerTransactions.Add(oTransaction);
                            _context.SaveChanges();
                        }
                        else
                        {
                            var oTransaction = new OwnerTransaction();
                            oTransaction.WalletId = oWallet.WalletId;
                            oTransaction.TransactionDate = DateTime.Now;
                            oTransaction.TransactionAmount = booking.TotalCost;
                            oTransaction.TransactionStatus = true;
                            oTransaction.TransactionRemain = 0;
                            oTransaction.TransactionContent = "Có Phòng đã được đặt thành công! Mã thanh toán: " + orderCode;
                            _context.OwnerTransactions.Add(oTransaction);
                            _context.SaveChanges();
                        }
                    }
                    return Ok(new { message = "Đã đặt phòng thành công! Mã thanh toán: " + orderCode });
                }
            }
            return Ok(new { message = "Đã đặt phòng không thành công!!!! " });
        }
    }
}
