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

        // Cloudflare R2 (AmazonS3Client)
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var accountId = config["CloudflareR2:AccountId"] ?? string.Empty;
            var accessKeyId = config["CloudflareR2:AccessKeyId"] ?? string.Empty;
            var secretAccessKey = config["CloudflareR2:SecretAccessKey"] ?? string.Empty;

            var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKeyId, secretAccessKey);
            var s3Config = new AmazonS3Config
            {
                ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true
            };

            return new AmazonS3Client(credentials, s3Config);
        });

        services.AddScoped<IStorageService, R2StorageService>();
        services.AddScoped<INotificationService, SmtpEmailService>();
        services.AddSingleton<ICacheService, EHisse.Infrastructure.Caching.CacheService>();

        return services;
    }
}
