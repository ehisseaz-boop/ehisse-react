using EHisseler.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
    public interface IAdminService
    {
        Task<List<User>> GetPendingUsersAsync();
        Task<List<SparePart>> GetPendingAdsAsync();
        Task<bool> ApproveUserAsync(int userId, int adminId);
        Task<bool> RejectUserAsync(int userId, string reason, int adminId);
        Task<bool> BlockUserAsync(int userId, string reason, int adminId);
        Task<bool> UnblockUserAsync(int userId, int adminId);
        Task<bool> ApproveAdAsync(int adId, int adminId);
        Task<bool> RejectAdAsync(int adId, string reason, int adminId);
        Task<(bool Success, string ErrorMessage)> RestoreAdAsync(int adId, int adminId);
        Task<List<User>> GetAllUsersAsync();
        Task<List<SparePart>> GetAllAdsAsync();
        Task<List<Category>> GetCategoriesAsync();
        Task<List<DeviceModel>> GetModelsAndBrandsAsync();
        Task<List<ActivityLog>> GetActivityLogsAsync(int count);
    }
}
