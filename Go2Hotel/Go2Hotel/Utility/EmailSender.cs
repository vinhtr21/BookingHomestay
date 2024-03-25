using System.Net.Mail;
using System.Net;

namespace Go2Hotel.Utility
{
    public class EmailSender
    {
        public void SendMail(string email, string sub, string body)
        {
            try
            {
                // Cấu hình SmtpClient
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("nammuoica50@gmail.com", "peqk mloo odkh ajss");
                smtpClient.EnableSsl = true;

                // Tạo đối tượng MailMessage
                MailMessage message = new MailMessage();
                message.From = new MailAddress("nammuoica50@gmail.com");
                message.To.Add(email);
                message.Subject = sub;
                message.Body = body;
                // Gửi email
                smtpClient.Send(message);

                Console.WriteLine("Password reset email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending password reset email: " + ex.Message);
            }
        }
    }
}
