
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
namespace MarketWebApp.Models.Entity

{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var smtpServer = emailSettings["SmtpServer"];
            var port = int.Parse(emailSettings["Port"]);
            var userName = emailSettings["UserName"];
            var password = emailSettings["Password"];
            var senderEmail = emailSettings["SenderEmail"];

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(senderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                client.Connect(smtpServer, port, false);
                client.Authenticate(userName, password);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
