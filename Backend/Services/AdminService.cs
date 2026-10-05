using EHisseler.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetPendingUsersAsync()
        {
            return await _context.Users
                .Where(u => u.ApprovalStatus == ApprovalStatus.Pending)
                .ToListAsync();
        }

        public async Task<List<SparePart>> GetPendingAdsAsync()
        {
            return await _context.SpareParts
                .Include(p => p.Seller)
                .ThenInclude(s => s.User)
                .Include(p => p.DeviceModel)
                .ThenInclude(m => m.Brand)
                .Include(p => p.PartType)
                .Include(p => p.Reviews)
                .Where(p => p.ModerationStatus == ApprovalStatus.Pending)
                .ToListAsync();
        }

        public async Task<bool> ApproveUserAsync(int userId, int adminId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.ApprovalStatus = ApprovalStatus.Approved;

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

        public async Task<bool> ApproveAdAsync(int adId, int adminId)
        {
            var ad = await _context.SpareParts.Include(p => p.Seller).ThenInclude(s => s.User).FirstOrDefaultAsync(p => p.Id == adId);
            if (ad == null) return false;

            var oldModerationStatus = ad.ModerationStatus;

            ad.ModerationStatus = ad.StockQuantity > 0 ? ApprovalStatus.Approved : ApprovalStatus.Approved;
            ad.RejectionReason = null; // Clear rejection reason on approval

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
                Details = $"Status {oldModerationStatus} -> {ad.ModerationStatus} olaraq dəyişdirildi"
            };
            _context.ActivityLogs.Add(log);

            if (ad.ModerationStatus == ApprovalStatus.Approved && oldModerationStatus != ApprovalStatus.Approved)
            {
                SendMarketingNotification(ad);
            }
            SendNotification(ad.Seller.Phone, ad.Seller.User.Email, $"Sizin '{ad.Title}' adlı elanınız təsdiqləndi.");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAdAsync(int adId, string reason, int adminId)
        {
            var ad = await _context.SpareParts.Include(p => p.Seller).ThenInclude(s => s.User).FirstOrDefaultAsync(p => p.Id == adId);
            if (ad == null) return false;

            var oldModerationStatus = ad.ModerationStatus;

            ad.ModerationStatus = ApprovalStatus.Rejected;
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
                Details = $"Status {oldModerationStatus} -> {ad.ModerationStatus} olaraq dəyişdirildi. Səbəb: {review.Reason}"
            };
            _context.ActivityLogs.Add(log);

            SendNotification(ad.Seller.Phone, ad.Seller.User.Email, $"Sizin '{ad.Title}' adlı elanınız rədd edildi. Səbəb: {review.Reason}");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string ErrorMessage)> RestoreAdAsync(int adId, int adminId)
        {
            var ad = await _context.SpareParts.Include(p => p.Seller).ThenInclude(s => s.User).FirstOrDefaultAsync(p => p.Id == adId);
            if (ad == null) return (false, "Not Found");

            if (ad.StockQuantity <= 0)
            {
                return (false, "Bərpa zamanı stokun müsbət olması tələb olunur.");
            }

            var oldModerationStatus = ad.ModerationStatus;

            ad.ModerationStatus = ApprovalStatus.Pending;
            ad.IsDeleted = false;
            ad.IsActive = true;

            var log = new ActivityLog
            {
                UserId = adminId,
                Action = "RestoreAd",
                EntityName = "SparePart",
                EntityId = ad.Id,
                Details = $"Elan bərpa edildi və yenidən yoxlamaya göndərildi. Status {oldModerationStatus} -> {ad.ModerationStatus}"
            };
            _context.ActivityLogs.Add(log);

            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<SparePart>> GetAllAdsAsync()
        {
            return await _context.SpareParts
                .Include(p => p.Seller)
                .Include(p => p.DeviceModel)
                .ThenInclude(m => m.Brand)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<List<DeviceModel>> GetModelsAndBrandsAsync()
        {
            return await _context.DeviceModels
                .Include(m => m.Brand)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<List<ActivityLog>> GetActivityLogsAsync(int count)
        {
            return await _context.ActivityLogs
                .OrderByDescending(l => l.Date)
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

        private void SendMarketingNotification(SparePart ad)
        {
            System.Console.WriteLine($"[Marketinq] Yeni məhsul satışa çıxarıldı: {ad.Title}");
        }
    }
}
