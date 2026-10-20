using EHisse.Application.DTOs;
using EHisse.Application.Interfaces;
using EHisse.Domain.Entities;
using EHisse.Domain.Enums;
using EHisse.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHisse.Persistence.Repositories
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardStats> GetDashboardStatsAsync()
        {
            var countUsers = await _context.Users.CountAsync();
            var pendingUsers = await _context.Users.CountAsync(u => u.ApprovalStatus == ApprovalStatus.Pending);
            var countParts = await _context.SpareParts.CountAsync(p => !p.IsDeleted);
            var pendingNewAds = await _context.SpareParts.CountAsync(p => p.ModerationStatus == ApprovalStatus.Pending && !p.IsUpdateSubmission && !p.IsDeleted);
            var pendingUpdateAds = await _context.SpareParts.CountAsync(p => p.ModerationStatus == ApprovalStatus.Pending && p.IsUpdateSubmission && !p.IsDeleted);
            var pendingRestockAds = await _context.SpareParts.CountAsync(p => p.ModerationStatus == ApprovalStatus.Pending && p.StockQuantity > 0 && !p.IsActive && !p.IsDeleted);
            var activeRequests = pendingUsers + pendingNewAds + pendingUpdateAds;

            return new AdminDashboardStats(
                countUsers,
                countParts,
                activeRequests,
                pendingUsers,
                pendingNewAds,
                pendingUpdateAds,
                pendingRestockAds
            );
        }

        public async Task<List<User>> GetPendingUsersAsync()
        {
            return await _context.Users
                .Include(u => u.SellerProfile)
                .Where(u => u.ApprovalStatus == ApprovalStatus.Pending)
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<SparePart>> GetPendingAdsAsync()
        {
            return await _context.SpareParts
                .Include(p => p.Seller)
                    .ThenInclude(s => s.User)
                .Include(p => p.DeviceModel)
                    .ThenInclude(m => m.Brand)
                .Include(p => p.DeviceModel)
                    .ThenInclude(m => m.Category)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.ModerationStatus == ApprovalStatus.Pending && !p.IsDeleted)
                .OrderByDescending(p => p.LastModifiedDate)
                .ToListAsync();
        }

        public async Task<bool> ApproveUserAsync(int userId, int adminId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.ApprovalStatus = ApprovalStatus.Approved;
            user.RejectionReason = null;

            if (!await _context.SellerProfiles.AnyAsync(s => s.UserId == user.Id))
            {
                var sellerProfile = new SellerProfile
                {
                    UserId = user.Id,
                    CompanyName = $"{user.FirstName} {user.LastName} ({user.Role})",
                    ContactPerson = $"{user.FirstName} {user.LastName}",
                    Phone = user.Phone,
                    WhatsAppNumber = user.Phone,
                    Rating = 5.0m
                };
                _context.SellerProfiles.Add(sellerProfile);
            }

            var history = new ApprovalHistory
            {
                TargetUserId = user.Id,
                AdminId = adminId,
                Status = ApprovalStatus.Approved,
                Reason = "Məlumatlar yoxlanıldı və təsdiqləndi"
            };
            _context.ApprovalHistories.Add(history);

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "İstifadəçi Təsdiqi",
                EntityName = "User",
                EntityId = user.Id,
                Details = $"{user.Username} adlı istifadəçi təsdiqləndi."
            };
            _context.ActivityLogs.Add(log);

            SendNotification(user.Phone, user.Email, "Sizin qeydiyyatınız uğurla təsdiqləndi.");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectUserAsync(int userId, string reason, int adminId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.ApprovalStatus = ApprovalStatus.Rejected;
            user.RejectionReason = string.IsNullOrEmpty(reason) ? "Məlumatlar yetərsiz və ya səhvdir" : reason;

            var history = new ApprovalHistory
            {
                TargetUserId = user.Id,
                AdminId = adminId,
                Status = ApprovalStatus.Rejected,
                Reason = user.RejectionReason
            };
            _context.ApprovalHistories.Add(history);

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "İstifadəçi İmtinası",
                EntityName = "User",
                EntityId = user.Id,
                Details = $"{user.Username} adlı istifadəçi rədd edildi. Səbəb: {history.Reason}"
            };
            _context.ActivityLogs.Add(log);

            SendNotification(user.Phone, user.Email, $"Sizin qeydiyyatınız rədd edildi. Səbəb: {history.Reason}");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BlockUserAsync(int userId, string reason, int adminId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsBlocked = true;

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "BlockUser",
                EntityName = "User",
                EntityId = user.Id,
                Details = $"İstifadəçi bloklandı. Səbəb: {reason}"
            };
            _context.ActivityLogs.Add(log);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(int userId, int adminId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsBlocked = false;

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "UnblockUser",
                EntityName = "User",
                EntityId = user.Id,
                Details = "İstifadəçi blokdan çıxarıldı."
            };
            _context.ActivityLogs.Add(log);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId, int adminId)
        {
            var user = await _context.Users
                .Include(u => u.SellerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Role == UserRole.Admin) return false;

            var username = user.Username;

            // Əlaqəli elanları və şəkilləri təmizləyirik
            if (user.SellerProfile != null)
            {
                var parts = await _context.SpareParts
                    .Include(p => p.Images)
                    .Include(p => p.Reviews)
                    .Where(p => p.SellerId == user.SellerProfile.Id)
                    .ToListAsync();

                foreach (var part in parts)
                {
                    if (part.Images.Any()) _context.SparePartImages.RemoveRange(part.Images);
                    if (part.Reviews.Any()) _context.SparePartReviews.RemoveRange(part.Reviews);
                }
                if (parts.Any()) _context.SpareParts.RemoveRange(parts);

                var sales = await _context.SparePartSaleAudits
                    .Where(s => s.SellerId == user.SellerProfile.Id)
                    .ToListAsync();
                if (sales.Any()) _context.SparePartSaleAudits.RemoveRange(sales);

                _context.SellerProfiles.Remove(user.SellerProfile);
            }

            // Təsdiq tarixçəsini təmizləyirik
            var histories = await _context.ApprovalHistories.Where(h => h.TargetUserId == user.Id).ToListAsync();
            if (histories.Any()) _context.ApprovalHistories.RemoveRange(histories);

            _context.Users.Remove(user);

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "DeleteUser",
                EntityName = "User",
                EntityId = userId,
                Details = $"{username} adlı istifadəçi admin tərəfindən sistemdən tam silindi."
            };
            _context.ActivityLogs.Add(log);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveAdAsync(int adId, int adminId)
        {
            var ad = await _context.SpareParts
                .Include(p => p.Seller)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.Id == adId);
            if (ad == null) return false;

            var oldModerationStatus = ad.ModerationStatus;

            ad.ModerationStatus = ApprovalStatus.Approved;
            ad.IsUpdateSubmission = false;
            // Biznes qaydası: Stok 0 olduqda elan aktiv ola BİLMƏZ. Yalnız stok > 0 olduqda aktivləşir.
            ad.IsActive = ad.StockQuantity > 0;
            ad.RejectionReason = null;

            var review = new SparePartReview
            {
                SparePartId = ad.Id,
                AdminId = adminId,
                Status = ad.ModerationStatus,
                Reason = "Təsdiqləndi"
            };
            _context.SparePartReviews.Add(review);

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "Status Change",
                EntityName = "SparePart",
                EntityId = ad.Id,
                Details = $"Status {oldModerationStatus} -> Approved. Aktivlik: {ad.IsActive}"
            };
            _context.ActivityLogs.Add(log);

            SendNotification(ad.Seller.Phone, ad.Seller.User.Email, $"Sizin '{ad.Title}' adlı elanınız təsdiqləndi.");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAdAsync(int adId, string reason, int adminId)
        {
            var ad = await _context.SpareParts
                .Include(p => p.Seller)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.Id == adId);
            if (ad == null) return false;

            var oldModerationStatus = ad.ModerationStatus;

            ad.ModerationStatus = ApprovalStatus.Rejected;
            ad.IsActive = false; // Rədd edilən elan heç bir halda aktiv ola bilməz
            ad.RejectionReason = string.IsNullOrEmpty(reason) ? "Qaydalarla ziddiyyət təşkil edir." : reason;

            var review = new SparePartReview
            {
                SparePartId = ad.Id,
                AdminId = adminId,
                Status = ApprovalStatus.Rejected,
                Reason = ad.RejectionReason
            };
            _context.SparePartReviews.Add(review);

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "Status Change",
                EntityName = "SparePart",
                EntityId = ad.Id,
                Details = $"Status {oldModerationStatus} -> Rejected. Səbəb: {review.Reason}"
            };
            _context.ActivityLogs.Add(log);

            SendNotification(ad.Seller.Phone, ad.Seller.User.Email, $"Sizin '{ad.Title}' adlı elanınız rədd edildi. Səbəb: {review.Reason}");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> RestoreAdAsync(int adId, int adminId)
        {
            var ad = await _context.SpareParts
                .Include(p => p.Seller)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.Id == adId);
            if (ad == null) return (false, "Not Found");

            if (ad.StockQuantity <= 0)
            {
                return (false, "Bərpa zamanı stok miqdarının 0-dan böyük olması tələb olunur.");
            }

            ad.ModerationStatus = ApprovalStatus.Approved;
            ad.IsDeleted = false;
            ad.IsActive = true;
            ad.IsUpdateSubmission = false;
            ad.RejectionReason = null;

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "RestoreAd",
                EntityName = "SparePart",
                EntityId = ad.Id,
                Details = $"Elan bərpa edildi və admin tərəfindən aktivləşdirildi (Stok: {ad.StockQuantity})."
            };
            _context.ActivityLogs.Add(log);

            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<bool> DeactivateAdAsync(int adId, int adminId, string reason)
        {
            var ad = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == adId);
            if (ad == null) return false;

            ad.IsActive = false;

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "DeactivateAd",
                EntityName = "SparePart",
                EntityId = ad.Id,
                Details = $"Elan admin tərəfindən deaktiv edildi. Səbəb: {reason}"
            };
            _context.ActivityLogs.Add(log);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.SellerProfile)
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.SellerProfile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<SparePart>> GetAllAdsAsync()
        {
            return await _context.SpareParts
                .Include(p => p.Seller)
                    .ThenInclude(s => s.User)
                .Include(p => p.DeviceModel)
                    .ThenInclude(m => m.Brand)
                .Include(p => p.DeviceModel)
                    .ThenInclude(m => m.Category)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        // ============================================
        // KATALOQ: KATEQORİYALAR
        // ============================================
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Category> CreateCategoryAsync(string name, int displayOrder)
        {
            var category = new Category
            {
                Name = name.Trim(),
                DisplayOrder = displayOrder,
                IsActive = true
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategoryAsync(int id, string name, int displayOrder, bool isActive)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.Name = name.Trim();
            category.DisplayOrder = displayOrder;
            category.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.IsActive = !category.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return (false, "Kateqoriya tapılmadı.");

            var relatedAdsCount = await _context.SpareParts
                .CountAsync(p => p.DeviceModel != null && p.DeviceModel.CategoryId == id && !p.IsDeleted);

            if (relatedAdsCount > 0)
            {
                return (false, $"Bu kateqoriyaya aid {relatedAdsCount} ədəd ehtiyat hissəsi (məhsul) mövcuddur! Məhsulu olan kateqoriya silinə bilməz.");
            }

            var relatedModels = await _context.DeviceModels.Where(m => m.CategoryId == id).ToListAsync();
            if (relatedModels.Any())
            {
                var modelIds = relatedModels.Select(m => m.Id).ToList();
                var partsWithModels = await _context.SpareParts.CountAsync(p => modelIds.Contains(p.DeviceModelId));
                if (partsWithModels > 0)
                {
                    return (false, $"Bu kateqoriyanın modellərinə aid {partsWithModels} ədəd ehtiyat hissəsi (məhsul) mövcuddur! Silinə bilməz.");
                }
                _context.DeviceModels.RemoveRange(relatedModels);
            }

            var categoryBrands = await _context.CategoryBrands.Where(cb => cb.CategoryId == id).ToListAsync();
            _context.CategoryBrands.RemoveRange(categoryBrands);

            var categoryPartTypes = await _context.CategoryPartTypes.Where(cp => cp.CategoryId == id).ToListAsync();
            _context.CategoryPartTypes.RemoveRange(categoryPartTypes);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        // ============================================
        // KATALOQ: BRENDLƏR
        // ============================================
        public async Task<List<DeviceBrand>> GetBrandsAsync()
        {
            return await _context.DeviceBrands
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<DeviceBrand> CreateBrandAsync(string name)
        {
            var brand = new DeviceBrand
            {
                Name = name.Trim(),
                IsActive = true
            };
            _context.DeviceBrands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<bool> UpdateBrandAsync(int id, string name, bool isActive)
        {
            var brand = await _context.DeviceBrands.FindAsync(id);
            if (brand == null) return false;

            brand.Name = name.Trim();
            brand.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleBrandAsync(int id)
        {
            var brand = await _context.DeviceBrands.FindAsync(id);
            if (brand == null) return false;

            brand.IsActive = !brand.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteBrandAsync(int id)
        {
            var brand = await _context.DeviceBrands.FindAsync(id);
            if (brand == null)
                return (false, "Brend tapılmadı.");

            var relatedAdsCount = await _context.SpareParts
                .CountAsync(p => p.DeviceModel != null && p.DeviceModel.BrandId == id && !p.IsDeleted);

            if (relatedAdsCount > 0)
            {
                return (false, $"Bu brendə aid {relatedAdsCount} ədəd ehtiyat hissəsi (məhsul) mövcuddur! Məhsulu olan brend silinə bilməz.");
            }

            var relatedModels = await _context.DeviceModels.Where(m => m.BrandId == id).ToListAsync();
            if (relatedModels.Any())
            {
                var modelIds = relatedModels.Select(m => m.Id).ToList();
                var partsWithModels = await _context.SpareParts.CountAsync(p => modelIds.Contains(p.DeviceModelId));
                if (partsWithModels > 0)
                {
                    return (false, $"Bu brendin modellərinə aid {partsWithModels} ədəd ehtiyat hissəsi (məhsul) mövcuddur! Silinə bilməz.");
                }
                _context.DeviceModels.RemoveRange(relatedModels);
            }

            var categoryBrands = await _context.CategoryBrands.Where(cb => cb.BrandId == id).ToListAsync();
            _context.CategoryBrands.RemoveRange(categoryBrands);

            _context.DeviceBrands.Remove(brand);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<List<CategoryBrand>> GetCategoryBrandsAsync()
        {
            return await _context.CategoryBrands.ToListAsync();
        }

        public async Task<bool> AssignBrandToCategoryAsync(int categoryId, int brandId)
        {
            if (await _context.CategoryBrands.AnyAsync(cb => cb.CategoryId == categoryId && cb.BrandId == brandId))
                return true;

            _context.CategoryBrands.Add(new CategoryBrand { CategoryId = categoryId, BrandId = brandId });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveBrandFromCategoryAsync(int categoryId, int brandId)
        {
            var item = await _context.CategoryBrands.FirstOrDefaultAsync(cb => cb.CategoryId == categoryId && cb.BrandId == brandId);
            if (item == null) return false;

            _context.CategoryBrands.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================================
        // KATALOQ: MODELLƏR
        // ============================================
        public async Task<List<DeviceModel>> GetModelsAndBrandsAsync()
        {
            return await _context.DeviceModels
                .Include(m => m.Brand)
                .Include(m => m.Category)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<DeviceModel> CreateModelAsync(string name, string? modelCode, int releaseYear, int categoryId, int brandId)
        {
            var model = new DeviceModel
            {
                Name = name.Trim(),
                ModelCode = modelCode ?? string.Empty,
                ReleaseYear = releaseYear,
                CategoryId = categoryId,
                BrandId = brandId,
                IsActive = true
            };
            _context.DeviceModels.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<bool> UpdateModelAsync(int id, string name, string? modelCode, int releaseYear, int categoryId, int brandId, bool isActive)
        {
            var model = await _context.DeviceModels.FindAsync(id);
            if (model == null) return false;

            model.Name = name.Trim();
            model.ModelCode = modelCode ?? string.Empty;
            model.ReleaseYear = releaseYear;
            model.CategoryId = categoryId;
            model.BrandId = brandId;
            model.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleModelAsync(int id)
        {
            var model = await _context.DeviceModels.FindAsync(id);
            if (model == null) return false;

            model.IsActive = !model.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteModelAsync(int id)
        {
            var model = await _context.DeviceModels.FindAsync(id);
            if (model == null)
                return (false, "Model tapılmadı.");

            var relatedAdsCount = await _context.SpareParts
                .CountAsync(p => p.DeviceModelId == id && !p.IsDeleted);

            if (relatedAdsCount > 0)
            {
                return (false, $"Bu modelə aid {relatedAdsCount} ədəd ehtiyat hissəsi (məhsul) mövcuddur! Məhsulu olan model silinə bilməz.");
            }

            _context.DeviceModels.Remove(model);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        // ============================================
        // KATALOQ: HİSSƏ NÖVLƏRİ VƏ MATRİS
        // ============================================
        public async Task<List<SparePartType>> GetPartTypesAsync()
        {
            return await _context.SparePartTypes
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<SparePartType> CreatePartTypeAsync(string name, string key)
        {
            var pt = new SparePartType
            {
                Name = name.Trim(),
                Key = string.IsNullOrWhiteSpace(key) ? name.Trim().ToLower().Replace(" ", "-") : key.Trim(),
                IsAccessory = false,
                IsPortable = false
            };
            _context.SparePartTypes.Add(pt);
            await _context.SaveChangesAsync();
            return pt;
        }

        public async Task<bool> UpdatePartTypeAsync(int id, string name, string key, bool isAccessory, bool isPortable)
        {
            var pt = await _context.SparePartTypes.FindAsync(id);
            if (pt == null) return false;

            pt.Name = name.Trim();
            pt.Key = key.Trim();
            pt.IsAccessory = isAccessory;
            pt.IsPortable = isPortable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryPartType>> GetCategoryPartTypesAsync()
        {
            return await _context.CategoryPartTypes.ToListAsync();
        }

        public async Task<bool> AssignPartTypeToCategoryAsync(int categoryId, int partTypeId)
        {
            if (await _context.CategoryPartTypes.AnyAsync(cp => cp.CategoryId == categoryId && cp.PartTypeId == partTypeId))
                return true;

            _context.CategoryPartTypes.Add(new CategoryPartType { CategoryId = categoryId, PartTypeId = partTypeId });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePartTypeFromCategoryAsync(int categoryId, int partTypeId)
        {
            var item = await _context.CategoryPartTypes.FirstOrDefaultAsync(cp => cp.CategoryId == categoryId && cp.PartTypeId == partTypeId);
            if (item == null) return false;

            _context.CategoryPartTypes.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> DeletePartTypeAsync(int id)
        {
            var pt = await _context.SparePartTypes.FindAsync(id);
            if (pt == null)
                return (false, "Hissə növü tapılmadı.");

            var relatedAdsCount = await _context.SpareParts
                .CountAsync(p => p.PartTypeId == id && !p.IsDeleted);

            if (relatedAdsCount > 0)
            {
                return (false, $"Bu hissə növünə aid {relatedAdsCount} ədəd ehtiyat hissəsi (məhsul) mövcuddur! Məhsulu olan hissə növü silinə bilməz.");
            }

            var categoryPartTypes = await _context.CategoryPartTypes.Where(cp => cp.PartTypeId == id).ToListAsync();
            _context.CategoryPartTypes.RemoveRange(categoryPartTypes);

            _context.SparePartTypes.Remove(pt);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        // ============================================
        // KEYFİYYƏT NÖVLƏRİ
        // ============================================
        public async Task<List<QualityType>> GetQualityTypesAsync()
        {
            return await _context.QualityTypes
                .OrderBy(q => q.DisplayOrder)
                .ToListAsync();
        }

        // ============================================
        // AUDIT VƏ JURNALLAR
        // ============================================
        public async Task<List<ActivityLog>> GetActivityLogsAsync(int count)
        {
            return await _context.ActivityLogs
                .OrderByDescending(l => l.Date)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<SparePartSaleAudit>> GetSaleAuditsAsync(int count)
        {
            return await _context.SparePartSaleAudits
                .Include(s => s.SparePart)
                .OrderByDescending(s => s.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        private void SendNotification(string phone, string? email, string message)
        {
            System.Console.WriteLine($"[SMS -> {phone}]: {message}");
            if (!string.IsNullOrEmpty(email))
            {
                System.Console.WriteLine($"[EMAIL -> {email}] (from: info@e-hisse.az): {message}");
            }
        }
    }
}
