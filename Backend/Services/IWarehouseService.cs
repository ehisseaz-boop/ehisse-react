using EHisseler.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
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
}
