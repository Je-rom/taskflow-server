using System.Net;
using System.Net.Mail;
using taskflow.Services.Interfaces;

namespace taskflow.Services.Impls
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
    
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    
        public string SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage();
                
            message.To.Add(new MailAddress("adurotimijoshua@gmail.com", "To Name"));
            message.From = new MailAddress("nedsisnedsis@gmail.com", "From Name");
            /*message.CC.Add(new MailAddress("cc@email.com", "CC Name"));
            message.Bcc.Add(new MailAddress("bcc@email.com", "BCC Name"));*/
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
    
            using (var client = new SmtpClient("sandbox.smtp.mailtrap.io"))
            {
                client.Port = 2525;
                client.Credentials = new NetworkCredential("81806e976d160d", "9a6c8b101cdbd6");
                client.EnableSsl = true;
                client.Send(message);
            };
                
            return "done";
        }
    }
}

