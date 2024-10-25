using api.DTOs; // Import the DTOs namespace
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace api.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;

            // Log the configuration values to confirm
            Console.WriteLine($"SMTP Host: {_smtpSettings.Host}");
            Console.WriteLine($"SMTP Port: {_smtpSettings.Port}");

            // Validate SMTP settings
            if (string.IsNullOrEmpty(_smtpSettings.Host))
            {
                throw new ArgumentException("SMTP Host is not configured. Please check your settings.");
            }

            if (_smtpSettings.Port <= 0)
            {
                throw new ArgumentException("SMTP Port must be a positive, non-zero value.");
            }

            if (string.IsNullOrEmpty(_smtpSettings.Username) || string.IsNullOrEmpty(_smtpSettings.Password))
            {
                throw new ArgumentException("SMTP Username and Password are required.");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_smtpSettings.Host)
                {
                    Port = _smtpSettings.Port,
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"[ERROR] Failed to send email: {ex.Message}");
                throw; // Re-throw to allow catching higher up
            }
        }
    }
}
