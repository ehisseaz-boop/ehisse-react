using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
    /// <summary>
    /// Azure Blob Storage implementasiyası.
    ///
    /// Performans strategiyası:
    ///   - DB-da yalnız BLOB PATH saxlanılır (tam URL yox).
    ///   - GetBulkUrls() heç bir HTTP sorğusu etmədən, yalnız URL konstruksiyası ilə
    ///     bütün şəkillərin URL-larını qaytarır — pagination-da N şəkil üçün 0 network round-trip.
    ///   - Container "public" olduqda URL birbaşa qurulur.
    ///   - Container "private" olduqda batch SAS token generasiyası edilir (hələlik public).
    /// </summary>
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly string _baseUrl;

        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["BlobStorage:ConnectionString"]
                ?? throw new InvalidOperationException("BlobStorage:ConnectionString konfiqurasiyasi tapilmadi.");

            _containerName = configuration["BlobStorage:ContainerName"] ?? "spare-parts";
            _blobServiceClient = new BlobServiceClient(connectionString);

            // Public URL bazası — container public olduqda istifadə olunur
            // Format: https://<account>.blob.core.windows.net/<container>
            _baseUrl = configuration["BlobStorage:BaseUrl"]
                ?? _blobServiceClient.Uri.ToString().TrimEnd('/') + $"/{_containerName}";
        }

        /// <inheritdoc/>
        public async Task<string> UploadAsync(IFormFile file, string folder)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Container yoxdursa yarat (public blob access ilə)
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var blobName = $"{folder}/{Guid.NewGuid():N}{extension}";

            var blobClient = containerClient.GetBlobClient(blobName);

            var headers = new BlobHttpHeaders
            {
                ContentType = GetContentType(extension)
            };

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers });

            // DB-a yalnız blob path saxlayırıq (base URL yox)
            return blobName;
        }

        /// <inheritdoc/>
        public string GetUrl(string blobPath)
        {
            if (string.IsNullOrEmpty(blobPath))
                return "/images/no-image.png";

            // Legacy wwwroot şəkilləri üçün birbaşa qaytaraq
            if (IsLegacyPath(blobPath))
                return blobPath;

            return $"{_baseUrl.TrimEnd('/')}/{blobPath}";
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Bu metod HEÇ BİR HTTP sorğusu etmir — yalnız string birləşməsi.
        /// 20 məhsul × 5 şəkil = 100 şəkil URL-ı anında hazır olur.
        /// </remarks>
        public Dictionary<string, string> GetBulkUrls(IEnumerable<string> blobPaths)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var baseUrl = _baseUrl.TrimEnd('/');

            foreach (var path in blobPaths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;

                if (result.ContainsKey(path))
                    continue;

                result[path] = IsLegacyPath(path)
                    ? path
                    : $"{baseUrl}/{path}";
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string blobPath)
        {
            if (string.IsNullOrEmpty(blobPath) || IsLegacyPath(blobPath))
                return;

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobPath);
            await blobClient.DeleteIfExistsAsync();
        }

        /// <inheritdoc/>
        public bool IsLegacyPath(string path)
        {
            return path.StartsWith("/images/") || path.StartsWith("images/");
        }

        private static string GetContentType(string extension) => extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png"            => "image/png",
            ".webp"           => "image/webp",
            ".gif"            => "image/gif",
            _                 => "application/octet-stream"
        };
    }
}
