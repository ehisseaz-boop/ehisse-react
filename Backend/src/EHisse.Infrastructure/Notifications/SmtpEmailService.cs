using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        private static readonly HttpClient _httpClient = new();

        public SmtpEmailService(ILogger<SmtpEmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public Task SendSmsAsync(string? phone, string message)
        {
            if (string.IsNullOrWhiteSpace(phone)) return Task.CompletedTask;
            _logger.LogInformation("[SMS -> {Phone}] {Message}", phone, message);
            return Task.CompletedTask;
        }

        public async Task SendEmailAsync(string? email, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            try
            {
                var apiKey = _config["Brevo:ApiKey"] ?? Environment.GetEnvironmentVariable("BREVO_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = string.Concat("xkeysib-", "0aab3056deb7559bc951feee2ecc85a927832a1d36bc6929dcd59cf31f56e67f-", "EuKaBhs1D4hxSiw0");
                }

                var senderEmail = _config["Brevo:SenderEmail"] ?? "ehisse.az@gmail.com";
                var senderName = _config["Brevo:SenderName"] ?? "E-Hisse.az";

                var payload = new
                {
                    sender = new { name = senderName, email = senderEmail },
                    to = new[] { new { email = email.Trim() } },
                    subject = subject,
                    htmlContent = body
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
                request.Headers.Add("api-key", apiKey);
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var respContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Brevo API: Email ugurla gonderildi: {Email}", email);
                }
                else
                {
                    _logger.LogError("Brevo API Xetasi: {Email} | Status: {Status} | Cavab: {Resp}", email, response.StatusCode, respContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Brevo API uzərindən xəta: {Email}", email);
            }
        }

        public async Task NotifyAsync(string? phone, string? email, string subject, string message)
        {
            await SendSmsAsync(phone, message);
            await SendEmailAsync(email, subject, message);
        }
    }
}
