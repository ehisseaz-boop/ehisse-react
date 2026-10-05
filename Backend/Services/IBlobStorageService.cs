using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
    /// <summary>
    /// Azure Blob Storage üzərindəki əməliyyatlar üçün interfeys.
    /// </summary>
    public interface IBlobStorageService
    {
        /// <summary>
        /// Faylı blob storage-a yükləyir və blob path-ini qaytarır.
        /// Path format: "parts/{sparePartId}/{guid}.jpg"
        /// </summary>
        Task<string> UploadAsync(IFormFile file, string folder);

        /// <summary>
        /// Bir blob path-i üçün public/SAS URL qaytarır (DB-da saxlanılan path-ə görə).
        /// </summary>
        string GetUrl(string blobPath);

        /// <summary>
        /// Bir neçə blob path-i üçün URL-ları EYNI ANDA qaytarır (HTTP sorğu olmadan,
        /// yalnız URL konstruksiyası ilə). Pagination və bulk load üçün optimallaşdırılıb.
        /// </summary>
        Dictionary<string, string> GetBulkUrls(IEnumerable<string> blobPaths);

        /// <summary>
        /// Blob-u storage-dan silir.
        /// </summary>
        Task DeleteAsync(string blobPath);

        /// <summary>
        /// Blob path-in seed/wwwroot şəkli olub olmadığını yoxlayır.
        /// </summary>
        bool IsLegacyPath(string path);
    }
}
