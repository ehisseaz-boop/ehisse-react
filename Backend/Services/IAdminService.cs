using EHisseler.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
    public record AdminDashboardStats(
        int CountUsers,
        int CountParts,
        int ActiveRequests,
        int PendingUsers,
        int PendingNewAds,
        int PendingUpdateAds,
        int PendingRestockAds
    );

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
}
