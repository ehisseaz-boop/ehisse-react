using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EHisseler.Web.Services
{
    /// <summary>
    /// SMS və email bildirişləri.
    /// Hazırkı implementasiya real provayder qoşulana qədər mesajları konsola və
    /// "notifications.log" faylına yazır. Real SMS/SMTP provayderi qoşmaq üçün
    /// yalnız bu interfeysin yeni implementasiyasını Program.cs-də qeydiyyatdan keçirmək kifayətdir.
    /// </summary>
    public interface INotificationService
    {
        Task SendSmsAsync(string? phone, string message);
        Task SendEmailAsync(string? email, string subject, string body);
        /// <summary>SMS həmişə, email isə yalnız ünvan varsa göndərilir.</summary>
        Task NotifyAsync(string? phone, string? email, string subject, string message);
    }

    public class LoggingNotificationService : INotificationService
    {
        private readonly ILogger<LoggingNotificationService> _logger;
        private static readonly object FileLock = new();
        private readonly string _logPath;

        public LoggingNotificationService(ILogger<LoggingNotificationService> logger)
        {
            _logger = logger;
            _logPath = Path.Combine(Directory.GetCurrentDirectory(), "notifications.log");
        }

        public Task SendSmsAsync(string? phone, string message)
        {
            if (string.IsNullOrWhiteSpace(phone)) return Task.CompletedTask;
            Write($"[SMS -> {phone}] {message}");
            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string? email, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(email)) return Task.CompletedTask;
            Write($"[EMAIL -> {email}] (from: info@e-hisse.az) Mövzu: {subject} | {body}");
            return Task.CompletedTask;
        }

        public async Task NotifyAsync(string? phone, string? email, string subject, string message)
        {
            await SendSmsAsync(phone, message);
            await SendEmailAsync(email, subject, message);
        }

        private void Write(string line)
        {
            var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {line}";
            _logger.LogInformation("{Notification}", entry);
            Console.WriteLine(entry);
            try
            {
                lock (FileLock) File.AppendAllText(_logPath, entry + Environment.NewLine);
            }
            catch { /* log faylı yazıla bilmirsə bildiriş axını dayanmamalıdır */ }
        }
    }
}
