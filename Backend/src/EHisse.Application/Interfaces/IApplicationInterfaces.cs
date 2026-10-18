using EHisse.Application.DTOs;
using EHisse.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace EHisse.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string originalFileName, string contentType, CancellationToken cancellationToken = default);
    Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileUrlOrKey, CancellationToken cancellationToken = default);
}

public interface IBlobStorageService
{
    Task<string> UploadAsync(IFormFile file, string folder);
    string GetUrl(string blobPath);
    Dictionary<string, string> GetBulkUrls(IEnumerable<string> blobPaths);
    Task DeleteAsync(string blobPath);
    bool IsLegacyPath(string path);
}

public interface INotificationService
{
    Task SendSmsAsync(string? phone, string message);
    Task SendEmailAsync(string? email, string subject, string body);
    Task NotifyAsync(string? phone, string? email, string subject, string message);
}

public interface ICatalogService
{
    Task<object> GetCategoriesAsync();
    Task<object> GetProductsAsync(string? search, int? categoryId, int? partTypeId);
}

public interface IWarehouseService
{
    Task<SellerProfile?> GetSellerProfileAsync(int userId);
    Task<List<SparePart>> GetMyPartsAsync(int sellerId);
    Task<List<Category>> GetCategoriesAsync();
    Task<List<QualityType>> GetQualityTypesAsync();
    Task<bool> SkuExistsAsync(string sku, int sellerId);
    Task<SparePart> AddPartAsync(SparePart part, int userId, int sellerId);
    Task<SparePart?> GetPartAsync(int id, int sellerId);
    Task<DeviceModel?> GetDeviceModelAsync(int deviceModelId);
    Task<(bool Success, string ErrorMessage, bool ConcurrencyError)> EditPartAsync(int id, SparePart updatedPart, int userId, int sellerId);
    Task<bool> UpdatePriceAsync(int id, decimal newPrice, int sellerId);
    Task<(bool Success, string ErrorMessage)> ToggleActiveAsync(int id, int sellerId);
    Task<bool> DeletePartAsync(int id, int sellerId);
    Task<(bool Success, string ErrorMessage, bool ConcurrencyError)> RecordSaleAsync(int id, int userId, int sellerId);
    Task<User?> GetUserWithSellerProfileAsync(int userId);
    Task<(bool Success, string ErrorMessage)> UpdateSettingsAsync(int userId, SellerProfile updatedProfile);
}

public interface IAdminService
{
    Task<AdminDashboardStats> GetDashboardStatsAsync();
    Task<List<User>> GetPendingUsersAsync();
    Task<List<SparePart>> GetPendingAdsAsync();
    Task<bool> ApproveUserAsync(int userId, int adminId);
    Task<bool> RejectUserAsync(int userId, string reason, int adminId);
    Task<bool> BlockUserAsync(int userId, string reason, int adminId);
    Task<bool> UnblockUserAsync(int userId, int adminId);
    Task<bool> ApproveAdAsync(int adId, int adminId);
    Task<bool> RejectAdAsync(int adId, string reason, int adminId);
    Task<(bool Success, string ErrorMessage)> RestoreAdAsync(int adId, int adminId);
    Task<bool> DeactivateAdAsync(int adId, int adminId, string reason);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<List<SparePart>> GetAllAdsAsync();

    // Kataloq - Kateqoriyalar
    Task<List<Category>> GetCategoriesAsync();
    Task<Category> CreateCategoryAsync(string name, int displayOrder);
    Task<bool> UpdateCategoryAsync(int id, string name, int displayOrder, bool isActive);
    Task<bool> ToggleCategoryAsync(int id);

    // Kataloq - Brendlər
    Task<List<DeviceBrand>> GetBrandsAsync();
    Task<DeviceBrand> CreateBrandAsync(string name);
    Task<bool> UpdateBrandAsync(int id, string name, bool isActive);
    Task<bool> ToggleBrandAsync(int id);
    Task<List<CategoryBrand>> GetCategoryBrandsAsync();
    Task<bool> AssignBrandToCategoryAsync(int categoryId, int brandId);
    Task<bool> RemoveBrandFromCategoryAsync(int categoryId, int brandId);

    // Kataloq - Modellər
    Task<List<DeviceModel>> GetModelsAndBrandsAsync();
    Task<DeviceModel> CreateModelAsync(string name, string? modelCode, int releaseYear, int categoryId, int brandId);
    Task<bool> UpdateModelAsync(int id, string name, string? modelCode, int releaseYear, int categoryId, int brandId, bool isActive);
    Task<bool> ToggleModelAsync(int id);

    // Kataloq - Hissə Növləri və Matris
    Task<List<SparePartType>> GetPartTypesAsync();
    Task<SparePartType> CreatePartTypeAsync(string name, string key);
    Task<bool> UpdatePartTypeAsync(int id, string name, string key, bool isAccessory, bool isPortable);
    Task<List<CategoryPartType>> GetCategoryPartTypesAsync();
    Task<bool> AssignPartTypeToCategoryAsync(int categoryId, int partTypeId);
    Task<bool> RemovePartTypeFromCategoryAsync(int categoryId, int partTypeId);

    // Keyfiyyət növləri
    Task<List<QualityType>> GetQualityTypesAsync();

    // Audit Jurnalları
    Task<List<ActivityLog>> GetActivityLogsAsync(int count);
    Task<List<SparePartSaleAudit>> GetSaleAuditsAsync(int count);
}
