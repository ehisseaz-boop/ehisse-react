using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EHisse.Application.Interfaces;

namespace EHisse.Infrastructure.Notifications
{
    public class SmtpEmailService : INotificationService
    {
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly IConfiguration _config;

        public SmtpEmailService(ILogger<SmtpEmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public Task SendSmsAsync(string? phone, string message)
        {
            // SMS təminatçısı inteqrasiyası hələ aktiv deyilsə console-a qeyd edilir
            if (string.IsNullOrWhiteSpace(phone)) return Task.CompletedTask;
            _logger.LogInformation("[SMS -> {Phone}] {Message}", phone, message);
            return Task.CompletedTask;
        }

        public async Task SendEmailAsync(string? email, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            try
            {
                var server = _config["SmtpSettings:Server"] ?? "smtp.gmail.com";
                var portStr = _config["SmtpSettings:Port"] ?? "587";
                int.TryParse(portStr, out int port);
                var senderEmail = _config["SmtpSettings:SenderEmail"] ?? "ehisse.az@gmail.com";
                var senderName = _config["SmtpSettings:SenderName"] ?? "E-Hisse.az";
                var password = _config["SmtpSettings:Password"]?.Replace(" ", "") ?? string.Empty;
                var enableSsl = bool.Parse(_config["SmtpSettings:EnableSsl"] ?? "true");

                using var client = new SmtpClient(server, port)
                {
                    Credentials = new NetworkCredential(senderEmail, password),
                    EnableSsl = enableSsl
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email uğurla göndərildi: {Email} | Mövzu: {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email göndərilərkən xəta baş verdi: {Email}", email);
            }
        }

        public async Task NotifyAsync(string? phone, string? email, string subject, string message)
        {
            await SendSmsAsync(phone, message);
            await SendEmailAsync(email, subject, message);
        }
    }
}
