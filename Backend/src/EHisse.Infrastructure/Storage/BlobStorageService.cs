using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using EHisse.Application.Interfaces;

namespace EHisse.Infrastructure.Storage
{
    /// <summary>
    /// Şəkillərin saxlanması xidməti.
    /// Azure Blob Storage mövcud olduqda ondan istifadə edir,
    /// Azure konfiqurasiya edilmədikdə və ya xəta verdikdə avtomatik olaraq
    /// yerli disk (wwwroot/uploads) rejiminə keçir (Fail-Safe Local Storage).
    /// Bu sayədə heç bir fayl yükləməsində sayt xəta verib çökmür.
    /// </summary>
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient? _blobServiceClient;
        private readonly string _containerName;
        private readonly string _baseUrl;
        private readonly bool _useLocalStorage;
        private readonly string _wwwrootUploadsFolder;
        private readonly ILogger<BlobStorageService>? _logger;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService>? logger = null)
        {
            _logger = logger;
            _wwwrootUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            var connectionString = configuration["BlobStorage:ConnectionString"];
            _containerName = configuration["BlobStorage:ContainerName"] ?? "spare-parts";
            _baseUrl = configuration["BlobStorage:BaseUrl"] ?? "";

            // Azure ConnectionString real olmadıqda yerli disk rejimi aktivləşir
            if (string.IsNullOrWhiteSpace(connectionString) ||
                connectionString.Contains("UseDevelopmentStorage=true", StringComparison.OrdinalIgnoreCase) ||
                connectionString.Contains("YOUR_ACCOUNT", StringComparison.OrdinalIgnoreCase) ||
                connectionString.StartsWith("local", StringComparison.OrdinalIgnoreCase))
            {
                _useLocalStorage = true;
                EnsureDirectory(_wwwrootUploadsFolder);
            }
            else
            {
                try
                {
                    _blobServiceClient = new BlobServiceClient(connectionString);
                    if (string.IsNullOrEmpty(_baseUrl))
                    {
                        _baseUrl = _blobServiceClient.Uri.ToString().TrimEnd('/') + $"/{_containerName}";
                    }
                    _useLocalStorage = false;
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Azure Blob Storage müştərisi başladıla bilmədi. Yerli disk fallback rejiminə keçilir.");
                    _useLocalStorage = true;
                    EnsureDirectory(_wwwrootUploadsFolder);
                }
            }
        }

        private static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <inheritdoc/>
        public async Task<string> UploadAsync(IFormFile file, string folder)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";

            if (_useLocalStorage || _blobServiceClient == null)
            {
                return await UploadToLocalAsync(file, folder, fileName);
            }

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobName = $"{folder}/{fileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                var headers = new BlobHttpHeaders
                {
                    ContentType = GetContentType(extension)
                };

                await using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers });

                return blobName;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Azure Blob-a yükləmə uğursuz oldu. Yerli diskə (fallback) yazılır.");
                return await UploadToLocalAsync(file, folder, fileName);
            }
        }

        private async Task<string> UploadToLocalAsync(IFormFile file, string folder, string fileName)
        {
            var cleanFolder = folder.Replace('\\', '/').Trim('/');
            var targetDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", cleanFolder.Replace('/', Path.DirectorySeparatorChar));
            EnsureDirectory(targetDir);

            var filePath = Path.Combine(targetDir, fileName);
            await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"uploads/{cleanFolder}/{fileName}";
        }

        /// <inheritdoc/>
        public string GetUrl(string blobPath)
        {
            if (string.IsNullOrEmpty(blobPath))
                return "/images/no-image.png";

            if (IsLegacyPath(blobPath))
                return blobPath.StartsWith("/") ? blobPath : $"/{blobPath}";

            if (_useLocalStorage || _blobServiceClient == null || blobPath.StartsWith("uploads/") || blobPath.StartsWith("/uploads/"))
                return blobPath.StartsWith("/") ? blobPath : $"/{blobPath}";

            return $"{_baseUrl.TrimEnd('/')}/{blobPath.TrimStart('/')}";
        }

        /// <inheritdoc/>
        public Dictionary<string, string> GetBulkUrls(IEnumerable<string> blobPaths)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var baseUrl = _baseUrl.TrimEnd('/');

            foreach (var path in blobPaths)
            {
                if (string.IsNullOrEmpty(path) || result.ContainsKey(path))
                    continue;

                if (IsLegacyPath(path) || _useLocalStorage || path.StartsWith("uploads/") || path.StartsWith("/uploads/"))
                {
                    result[path] = path.StartsWith("/") ? path : $"/{path}";
                }
                else
                {
                    result[path] = $"{baseUrl}/{path.TrimStart('/')}";
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string blobPath)
        {
            if (string.IsNullOrEmpty(blobPath))
                return;

            if (blobPath.StartsWith("uploads/") || blobPath.StartsWith("/uploads/"))
            {
                try
                {
                    var clean = blobPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", clean);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Yerli fayl silinərkən xəta: {Path}", blobPath);
                }
                return;
            }

            if (!_useLocalStorage && _blobServiceClient != null)
            {
                try
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                    var blobClient = containerClient.GetBlobClient(blobPath);
                    await blobClient.DeleteIfExistsAsync();
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Azure blob silinərkən xəta: {Path}", blobPath);
                }
            }
        }

        /// <inheritdoc/>
        public bool IsLegacyPath(string path)
        {
            return path.StartsWith("/images/") || path.StartsWith("images/") ||
                   path.StartsWith("/uploads/") || path.StartsWith("uploads/");
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
