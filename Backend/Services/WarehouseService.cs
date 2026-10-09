using EHisseler.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly AppDbContext _context;

        public WarehouseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SellerProfile?> GetSellerProfileAsync(int userId)
        {
            return await _context.SellerProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<List<SparePart>> GetMyPartsAsync(int sellerId)
        {
            return await _context.SpareParts
                .Include(p => p.DeviceModel)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Reviews)
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<List<QualityType>> GetQualityTypesAsync()
        {
            return await _context.QualityTypes.ToListAsync();
        }

        public async Task<bool> SkuExistsAsync(string sku, int sellerId)
        {
            return await _context.SpareParts.AnyAsync(p => p.SKU == sku && p.SellerId == sellerId && !p.IsDeleted);
        }

        public async Task<SparePart> AddPartAsync(SparePart part, int userId, int sellerId)
        {
            if (string.IsNullOrEmpty(part.SKU))
            {
                part.SKU = $"SKU-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            }
            
            part.SellerId = sellerId;
            part.CreatedDate = DateTime.Now;
            part.LastModifiedDate = DateTime.Now;
            part.ModerationStatus = ApprovalStatus.Pending;
            part.IsActive = true;
            part.IsDeleted = false;
            
            _context.Add(part);
            
            var log = new ActivityLog
            {
                UserId = userId,
                Action = "CreateAd",
                EntityName = "SparePart",
                Details = "Yeni elan yaradıldı",
                Date = DateTime.Now
            };
            _context.ActivityLogs.Add(log);

            await _context.SaveChangesAsync();
            return part;
        }

        public async Task<SparePart?> GetPartAsync(int id, int sellerId)
        {
            return await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId && !p.IsDeleted);
        }

        public async Task<DeviceModel?> GetDeviceModelAsync(int deviceModelId)
        {
            return await _context.DeviceModels.FirstOrDefaultAsync(m => m.Id == deviceModelId);
        }

        public async Task<(bool Success, string ErrorMessage, bool ConcurrencyError)> EditPartAsync(int id, SparePart updatedPart, int userId, int sellerId)
        {
            var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId && !p.IsDeleted);
            if (part == null) return (false, "Not Found", false);

            bool wasOutOfStock = part.StockQuantity <= 0;
            bool isRestocking = wasOutOfStock && updatedPart.StockQuantity > 0;
            bool requiresApproval = false;

            if (part.Title != updatedPart.Title ||
                part.DeviceModelId != updatedPart.DeviceModelId ||
                part.PartTypeId != updatedPart.PartTypeId ||
                part.QualityTypeId != updatedPart.QualityTypeId ||
                part.Description != updatedPart.Description ||
                part.Price != updatedPart.Price ||
                isRestocking)
            {
                requiresApproval = true;
            }

            part.DeviceModelId = updatedPart.DeviceModelId;
            part.PartTypeId = updatedPart.PartTypeId;
            part.QualityTypeId = updatedPart.QualityTypeId;
            part.Title = updatedPart.Title;
            part.Description = updatedPart.Description;
            part.Price = updatedPart.Price;
            part.Currency = "AZN"; // Biznes qaydası: Valyuta yalnız AZN
            part.StockQuantity = updatedPart.StockQuantity;
            part.MinOrderQuantity = updatedPart.MinOrderQuantity;
            part.WarrantyMonths = updatedPart.WarrantyMonths;
            if (!string.IsNullOrEmpty(updatedPart.SKU))
            {
                part.SKU = updatedPart.SKU;
            }
            part.DiscountPrice = updatedPart.DiscountPrice;
            part.Latitude = updatedPart.Latitude;
            part.Longitude = updatedPart.Longitude;
            part.LocationAddress = updatedPart.LocationAddress;
            part.LastModifiedDate = DateTime.Now;

            if (requiresApproval)
            {
                part.ModerationStatus = ApprovalStatus.Pending;
                part.IsActive = false;
                part.IsUpdateSubmission = true;
            }

            // Stok 0 olduqda elan mütləq deaktiv olmalıdır
            if (part.StockQuantity <= 0)
            {
                part.IsActive = false;
            }

            try
            {
                _context.Update(part);
                
                var log = new ActivityLog
                {
                    UserId = userId,
                    Action = "UpdateAd",
                    EntityName = "SparePart",
                    EntityId = part.Id,
                    Details = "Elan yeniləndi",
                    Date = DateTime.Now
                };
                _context.ActivityLogs.Add(log);

                await _context.SaveChangesAsync();
                return (true, string.Empty, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                return (false, "Concurrency error", true);
            }
        }

        public async Task<bool> UpdatePriceAsync(int id, decimal newPrice, int sellerId)
        {
            var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId && !p.IsDeleted);
            if (part == null) return false;

            part.Price = newPrice;
            part.LastModifiedDate = DateTime.Now;

            _context.Update(part);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> ToggleActiveAsync(int id, int sellerId)
        {
            var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId && !p.IsDeleted);
            if (part == null) return (false, "Not Found");

            if (part.IsActive)
            {
                part.IsActive = false;
                part.ModerationStatus = ApprovalStatus.Approved;
                part.LastModifiedDate = DateTime.Now;

                _context.Update(part);
                await _context.SaveChangesAsync();
                return (true, string.Empty);
            }
            else
            {
                return (false, "Elanı yenidən aktivləşdirmək yalnız administrator tərəfindən edilə bilər.");
            }
        }

        public async Task<bool> DeletePartAsync(int id, int sellerId)
        {
            var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId && !p.IsDeleted);
            if (part == null) return false;

            part.IsDeleted = true;
            part.IsActive = false;
            part.LastModifiedDate = DateTime.Now;

            _context.Update(part);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string ErrorMessage, bool ConcurrencyError)> RecordSaleAsync(int id, int userId, int sellerId)
        {
            try 
            {
                var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId && !p.IsDeleted);
                
                if (part == null || part.StockQuantity <= 0)
                {
                    return (false, "Məhsul tapılmadı və ya stok bitib.", false);
                }

                var previousStock = part.StockQuantity;
                part.StockQuantity -= 1;

                if (part.StockQuantity == 0)
                {
                    part.ModerationStatus = ApprovalStatus.Approved;
                    part.IsActive = false;
                }

                var sale = new SparePartSaleAudit {
                    SparePartId = part.Id,
                    SellerId = userId,
                    Quantity = 1,
                    PreviousStock = previousStock,
                    NewStock = part.StockQuantity,
                    Timestamp = DateTime.Now
                };

                _context.SparePartSaleAudits.Add(sale);
                await _context.SaveChangesAsync();
                return (true, string.Empty, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                return (false, "Concurrency error", true);
            }
            catch
            {
                return (false, "Satış zamanı xəta baş verdi.", false);
            }
        }

        public async Task<User?> GetUserWithSellerProfileAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.SellerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateSettingsAsync(int userId, SellerProfile updatedProfile)
        {
            var user = await _context.Users
                .Include(u => u.SellerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.SellerProfile == null) return (false, "Not Found");

            if (!string.IsNullOrWhiteSpace(updatedProfile.WhatsAppNumber))
            {
                if (!EHisseler.Web.Models.PhoneNumberHelper.TryNormalizeAzNumber(updatedProfile.WhatsAppNumber, out string _, out string _))
                {
                    return (false, "WhatsApp nömrəsi düzgün formatda deyil.");
                }
                user.SellerProfile.WhatsAppNumber = updatedProfile.WhatsAppNumber;
            }
            else
            {
                user.SellerProfile.WhatsAppNumber = null;
            }

            user.SellerProfile.ShowContactInfoInAds = updatedProfile.ShowContactInfoInAds;
            if (!string.IsNullOrWhiteSpace(updatedProfile.CompanyName)) user.SellerProfile.CompanyName = updatedProfile.CompanyName.Trim();
            if (!string.IsNullOrWhiteSpace(updatedProfile.ContactPerson)) user.SellerProfile.ContactPerson = updatedProfile.ContactPerson.Trim();
            user.SellerProfile.Address = updatedProfile.Address;
            
            if (user.SellerProfile.Phone != updatedProfile.Phone)
            {
                if (!EHisseler.Web.Models.PhoneNumberHelper.TryNormalizeAzNumber(updatedProfile.Phone, out string _, out string normalizedTel))
                {
                    return (false, "Mobil nömrə düzgün formatda deyil.");
                }
                user.SellerProfile.Phone = normalizedTel;
                user.SellerProfile.IsPhoneVerified = false;
            }
            
            _context.Update(user.SellerProfile);
            await _context.SaveChangesAsync();
            
            return (true, string.Empty);
        }
    }
}
