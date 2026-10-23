using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EHisse.Application.Interfaces;

namespace EHisse.Infrastructure.Storage;

public class R2StorageService : IStorageService
{
    private readonly IAmazonS3? _s3Client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<R2StorageService> _logger;
    private readonly string _bucketName;
    private readonly string _publicDomain;
    private readonly bool _isConfigured;
    private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    public R2StorageService(
        IConfiguration configuration,
        ILogger<R2StorageService> logger,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _bucketName = _configuration["CloudflareR2:BucketName"] 
            ?? Environment.GetEnvironmentVariable("CloudflareR2__BucketName") 
            ?? string.Empty;

        var publicDomain = _configuration["CloudflareR2:PublicDomain"] 
            ?? Environment.GetEnvironmentVariable("CloudflareR2__PublicDomain") 
            ?? string.Empty;
        _publicDomain = publicDomain.TrimEnd('/');

        var accountId = _configuration["CloudflareR2:AccountId"] ?? Environment.GetEnvironmentVariable("CloudflareR2__AccountId") ?? string.Empty;
        var accessKeyId = _configuration["CloudflareR2:AccessKeyId"] ?? Environment.GetEnvironmentVariable("CloudflareR2__AccessKeyId") ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(_bucketName) && 
            !_bucketName.Contains("YOUR_") &&
            !string.IsNullOrWhiteSpace(accountId) &&
            !accountId.Contains("YOUR_") &&
            !string.IsNullOrWhiteSpace(accessKeyId) &&
            !accessKeyId.Contains("YOUR_"))
        {
            try
            {
                _s3Client = serviceProvider.GetService<IAmazonS3>();
                _isConfigured = _s3Client != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cloudflare R2 s3Client alına bilmədi.");
                _isConfigured = false;
            }
        }
        else
        {
            _isConfigured = false;
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string originalFileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (!_isConfigured || _s3Client == null)
        {
            throw new InvalidOperationException("Cloudflare R2 konfiqurasiya edilməyib və ya əlçatan deyil.");
        }

        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("Fayl axını boşdur.", nameof(fileStream));
        }

        var extension = Path.GetExtension(originalFileName);
        var uniqueFileName = $"{Guid.NewGuid():N}{extension}";

        if (string.IsNullOrWhiteSpace(contentType) || contentType == "application/octet-stream")
        {
            if (_contentTypeProvider.TryGetContentType(originalFileName, out var resolvedContentType))
            {
                contentType = resolvedContentType;
            }
            else
            {
                contentType = "application/octet-stream";
            }
        }

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = uniqueFileName,
            InputStream = fileStream,
            ContentType = contentType,
            DisablePayloadSigning = true // Cloudflare R2 üçün tövsiyə olunan optimizasiya
        };

        _logger.LogInformation("Fayl Cloudflare R2-yə yüklənir: {Key}, ContentType: {ContentType}", uniqueFileName, contentType);
        
        var response = await _s3Client.PutObjectAsync(putRequest, cancellationToken);

        if ((int)response.HttpStatusCode >= 200 && (int)response.HttpStatusCode < 300)
        {
            var publicUrl = string.IsNullOrWhiteSpace(_publicDomain)
                ? uniqueFileName
                : $"{_publicDomain}/{uniqueFileName}";

            _logger.LogInformation("Fayl uğurla yükləndi: {PublicUrl}", publicUrl);
            return publicUrl;
        }

        throw new InvalidOperationException($"Cloudflare R2 fayl yükləmə xətası. Status code: {response.HttpStatusCode}");
    }

    public async Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Yüklənəcək fayl seçilməyib və ya boşdur.", nameof(file));
        }

        if (!_isConfigured || _s3Client == null)
        {
            throw new InvalidOperationException("Cloudflare R2 konfiqurasiya edilməyib və ya əlçatan deyil.");
        }

        await using var stream = file.OpenReadStream();
        return await UploadFileAsync(stream, file.FileName, file.ContentType, cancellationToken);
    }

    public async Task<bool> DeleteFileAsync(string fileUrlOrKey, CancellationToken cancellationToken = default)
    {
        if (!_isConfigured || _s3Client == null || string.IsNullOrWhiteSpace(fileUrlOrKey))
        {
            return false;
        }

        var key = fileUrlOrKey;
        if (!string.IsNullOrWhiteSpace(_publicDomain) && key.StartsWith(_publicDomain, StringComparison.OrdinalIgnoreCase))
        {
            key = key.Substring(_publicDomain.Length).TrimStart('/');
        }
        else if (Uri.TryCreate(fileUrlOrKey, UriKind.Absolute, out var uri))
        {
            key = uri.AbsolutePath.TrimStart('/');
        }

        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
            _logger.LogInformation("Fayl Cloudflare R2-dən silindi: {Key}", key);
            return (int)response.HttpStatusCode >= 200 && (int)response.HttpStatusCode < 300;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cloudflare R2-dən fayl silinərkən xəta baş verdi: {Key}", key);
            return false;
        }
    }
}
