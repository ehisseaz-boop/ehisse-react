using Amazon.S3;
using EHisse.Application.Interfaces;
using EHisse.Infrastructure.Notifications;
using EHisse.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EHisse.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Azure Blob Storage servisi
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        // Cloudflare R2 (AmazonS3Client) - konfiqurasiya yoxdursa null qaytarır, tətbiqi çökdürmür
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var accountId = config["CloudflareR2:AccountId"] ?? Environment.GetEnvironmentVariable("CloudflareR2__AccountId") ?? string.Empty;
            var accessKeyId = config["CloudflareR2:AccessKeyId"] ?? Environment.GetEnvironmentVariable("CloudflareR2__AccessKeyId") ?? string.Empty;
            var secretAccessKey = config["CloudflareR2:SecretAccessKey"] ?? Environment.GetEnvironmentVariable("CloudflareR2__SecretAccessKey") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(accountId) ||
                string.IsNullOrWhiteSpace(accessKeyId) ||
                string.IsNullOrWhiteSpace(secretAccessKey) ||
                accountId.Contains("YOUR_") ||
                accessKeyId.Contains("YOUR_"))
            {
                return null!;
            }

            try
            {
                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKeyId, secretAccessKey);
                var s3Config = new AmazonS3Config
                {
                    ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
                    ForcePathStyle = true
                };

                return new AmazonS3Client(credentials, s3Config);
            }
            catch
            {
                return null!;
            }
        });

        services.AddScoped<IStorageService, R2StorageService>();
        services.AddScoped<INotificationService, SmtpEmailService>();

        return services;
    }
}
