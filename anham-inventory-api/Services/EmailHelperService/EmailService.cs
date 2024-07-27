using System.Net;
using System.Net.Mail;

namespace anham_inventory_api.Services.EmailHelperService
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(string sendTo, string subject, string body)
        {
            try
            {
                string EmailServer = _configuration.GetSection("Email").GetSection("EmailServer").Value;
                int port = Convert.ToInt32(_configuration.GetSection("Email").GetSection("Port").Value);
                string fromEmail = _configuration.GetSection("Email").GetSection("FromEmail").Value;
                string password = _configuration.GetSection("Email").GetSection("Password").Value;

                // configure from email
                var fromAddress = new MailAddress(fromEmail);
                //configure smtp configuration
                var smtp = new SmtpClient();
                smtp.Host = EmailServer;
                smtp.Port = port;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(fromAddress.Address, password);
                smtp.Timeout = 100000;

                var message = new MailMessage();
                message.From = fromAddress;
                message.To.Add(new MailAddress(sendTo));

                message.Subject = subject;
                message.Body = body;

                smtp.Send(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
