using EHisse.Domain.Entities;
using EHisse.Domain.Enums;
using EHisse.Application.DTOs;
using EHisse.Application.Common;
using EHisse.Persistence.Context;
using EHisse.Persistence.Repositories;
using EHisse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisse.WebApi.Controllers
{
    public record ReasonInput(string? Reason);
    public record CategoryInput(string Name, int DisplayOrder = 0, bool IsActive = true);
    public record BrandInput(string Name, bool IsActive = true);
    public record AssignBrandInput(int CategoryId, int BrandId);
    public record ModelInput(string Name, string? ModelCode, int ReleaseYear, int CategoryId, int BrandId, bool IsActive = true);
    public record PartTypeInput(string Name, string? Key, bool IsAccessory = false, bool IsPortable = false);
    public record AssignPartTypeInput(int CategoryId, int PartTypeId);

    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private int AdminId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private static object MapUser(User u) => new
        {
            id = u.Id,
            username = u.Username,
            firstName = u.FirstName,
            lastName = u.LastName,
            fullName = $"{u.FirstName} {u.LastName}".Trim(),
            email = u.Email,
            phone = u.Phone,
            role = u.Role.ToString(),
            approvalStatus = u.ApprovalStatus.ToString(),
            rejectionReason = u.RejectionReason,
            isBlocked = u.IsBlocked,
            createdDate = u.CreatedDate
        };

        private static object MapAd(SparePart p) => new
        {
            id = p.Id,
            title = p.Title,
            description = p.Description,
            sku = p.SKU,
            price = p.Price,
            currency = p.Currency,
            stockQuantity = p.StockQuantity,
            minOrderQuantity = p.MinOrderQuantity,
            warrantyMonths = p.WarrantyMonths,
            isActive = p.IsActive,
            isDeleted = p.IsDeleted,
            moderationStatus = p.ModerationStatus.ToString(),
            rejectionReason = p.RejectionReason,
            isUpdateSubmission = p.IsUpdateSubmission,
            brandName = p.DeviceModel?.Brand?.Name,
            modelName = p.DeviceModel?.Name,
            categoryName = p.DeviceModel?.Category?.Name,
            partTypeName = p.PartType?.Name,
            qualityTypeName = p.QualityType?.Name,
            sellerName = p.Seller?.CompanyName ?? p.Seller?.ContactPerson,
            sellerPhone = p.Seller?.Phone ?? p.Seller?.User?.Phone,
            sellerWhatsApp = p.Seller?.WhatsAppNumber,
            images = p.Images?.Select(i => new { id = i.Id, imageUrl = i.ImageUrl, isMain = i.IsMain }).ToList(),
            createdDate = p.CreatedDate,
            lastModifiedDate = p.LastModifiedDate
        };

        // GET api/Admin/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        // GET api/Admin/Index (Frontend uyğunluğu üçün)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        // GET api/Admin/Requests — gözləyən istifadəçilər + yeni elanlar + redaktə sorğuları + stok bərpa sorğuları
        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            var pendingUsers = await _adminService.GetPendingUsersAsync();
            var pendingAds = await _adminService.GetPendingAdsAsync();

            var newAds = pendingAds.Where(p => !p.IsUpdateSubmission).ToList();
            var updateAds = pendingAds.Where(p => p.IsUpdateSubmission).ToList();

            return Ok(new
            {
                pendingUsers = pendingUsers.Select(MapUser),
                newAdRequests = newAds.Select(MapAd),
                updateAdRequests = updateAds.Select(MapAd)
            });
        }

        [HttpPost("{userId:int}")]
        public async Task<IActionResult> ApproveUser(int userId)
            => await _adminService.ApproveUserAsync(userId, AdminId) 
                ? Ok(new { success = true, message = "İstifadəçi qeydiyyatı təsdiqləndi." }) 
                : NotFound(new { message = "İstifadəçi tapılmadı." });

        [HttpPost("{userId:int}")]
        public async Task<IActionResult> RejectUser(int userId, [FromBody] ReasonInput input)
            => await _adminService.RejectUserAsync(userId, input.Reason ?? string.Empty, AdminId) 
                ? Ok(new { success = true, message = "İstifadəçi qeydiyyatı rədd edildi." }) 
                : NotFound(new { message = "İstifadəçi tapılmadı." });

        [HttpPost("{userId:int}")]
        public async Task<IActionResult> BlockUser(int userId, [FromBody] ReasonInput input)
            => await _adminService.BlockUserAsync(userId, input.Reason ?? string.Empty, AdminId) 
                ? Ok(new { success = true, message = "İstifadəçi bloklandı." }) 
                : NotFound(new { message = "İstifadəçi tapılmadı." });

        [HttpPost("{userId:int}")]
        public async Task<IActionResult> UnblockUser(int userId)
            => await _adminService.UnblockUserAsync(userId, AdminId) 
                ? Ok(new { success = true, message = "İstifadəçi blokdan çıxarıldı." }) 
                : NotFound(new { message = "İstifadəçi tapılmadı." });

        // GET api/Admin/PendingAds
        [HttpGet]
        public async Task<IActionResult> PendingAds()
            => Ok((await _adminService.GetPendingAdsAsync()).Select(MapAd));

        [HttpPost("{adId:int}")]
        public async Task<IActionResult> ApproveAd(int adId)
            => await _adminService.ApproveAdAsync(adId, AdminId) 
                ? Ok(new { success = true, message = "Elan təsdiqləndi." }) 
                : NotFound(new { message = "Elan tapılmadı." });

        [HttpPost("{adId:int}")]
        public async Task<IActionResult> RejectAd(int adId, [FromBody] ReasonInput input)
            => await _adminService.RejectAdAsync(adId, input.Reason ?? string.Empty, AdminId) 
                ? Ok(new { success = true, message = "Elan rədd edildi." }) 
                : NotFound(new { message = "Elan tapılmadı." });

        [HttpPost("{adId:int}")]
        public async Task<IActionResult> RestoreAd(int adId)
        {
            var result = await _adminService.RestoreAdAsync(adId, AdminId);
            if (!result.Success && result.ErrorMessage == "Not Found") return NotFound(new { message = "Elan tapılmadı." });
            if (!result.Success) return BadRequest(new { message = result.ErrorMessage });
            return Ok(new { success = true, message = "Elan bərpa edildi və aktivləşdirildi." });
        }

        [HttpPost("{adId:int}")]
        public async Task<IActionResult> DeactivateAd(int adId, [FromBody] ReasonInput input)
            => await _adminService.DeactivateAdAsync(adId, AdminId, input.Reason ?? "Admin tərəfindən deaktiv edildi")
                ? Ok(new { success = true, message = "Elan deaktiv edildi." })
                : NotFound(new { message = "Elan tapılmadı." });

        [HttpGet]
        public async Task<IActionResult> AllUsers()
            => Ok((await _adminService.GetAllUsersAsync()).Select(MapUser));

        [HttpGet]
        public async Task<IActionResult> AllAds()
            => Ok((await _adminService.GetAllAdsAsync()).Select(MapAd));

        // ============================================
        // KATALOQ: KATEQORİYALAR
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Categories()
            => Ok((await _adminService.GetCategoriesAsync()).Select(c => new 
            { 
                id = c.Id, 
                name = c.Name, 
                displayOrder = c.DisplayOrder, 
                isActive = c.IsActive 
            }));

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message = "Ad boş ola bilməz." });
            var cat = await _adminService.CreateCategoryAsync(input.Name, input.DisplayOrder);
            return Ok(new { success = true, id = cat.Id, message = "Kateqoriya yaradıldı." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message = "Ad boş ola bilməz." });
            var success = await _adminService.UpdateCategoryAsync(id, input.Name, input.DisplayOrder, input.IsActive);
            return success ? Ok(new { success = true, message = "Kateqoriya yeniləndi." }) : NotFound(new { message = "Kateqoriya tapılmadı." });
        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> ToggleCategory(int id)
        {
            var success = await _adminService.ToggleCategoryAsync(id);
            return success ? Ok(new { success = true }) : NotFound(new { message = "Kateqoriya tapılmadı." });
        }

        // ============================================
        // KATALOQ: BRENDLƏR
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Brands()
            => Ok((await _adminService.GetBrandsAsync()).Select(b => new 
            { 
                id = b.Id, 
                name = b.Name, 
                isActive = b.IsActive 
            }));

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] BrandInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message = "Brend adı boş ola bilməz." });
            var brand = await _adminService.CreateBrandAsync(input.Name);
            return Ok(new { success = true, id = brand.Id, message = "Brend əlavə edildi." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] BrandInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message = "Brend adı boş ola bilməz." });
            var success = await _adminService.UpdateBrandAsync(id, input.Name, input.IsActive);
            return success ? Ok(new { success = true, message = "Brend yeniləndi." }) : NotFound(new { message = "Brend tapılmadı." });
        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> ToggleBrand(int id)
        {
            var success = await _adminService.ToggleBrandAsync(id);
            return success ? Ok(new { success = true }) : NotFound(new { message = "Brend tapılmadı." });
        }

        [HttpGet]
        public async Task<IActionResult> CategoryBrands()
            => Ok((await _adminService.GetCategoryBrandsAsync()).Select(cb => new { categoryId = cb.CategoryId, brandId = cb.BrandId }));

        [HttpPost]
        public async Task<IActionResult> AssignBrand([FromBody] AssignBrandInput input)
        {
            var success = await _adminService.AssignBrandToCategoryAsync(input.CategoryId, input.BrandId);
            return success ? Ok(new { success = true }) : BadRequest(new { message = "Əlaqələndirmə baş tutmadı." });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBrand([FromBody] AssignBrandInput input)
        {
            var success = await _adminService.RemoveBrandFromCategoryAsync(input.CategoryId, input.BrandId);
            return success ? Ok(new { success = true }) : NotFound(new { message = "Əlaqə tapılmadı." });
        }

        // ============================================
        // KATALOQ: MODELLƏR
        // ============================================
        [HttpGet]
        public async Task<IActionResult> ModelsAndBrands()
            => Ok((await _adminService.GetModelsAndBrandsAsync()).Select(m => new
            {
                id = m.Id,
                name = m.Name,
                modelCode = m.ModelCode,
                releaseYear = m.ReleaseYear,
                isActive = m.IsActive,
                categoryId = m.CategoryId,
                categoryName = m.Category?.Name,
                brandId = m.BrandId,
                brandName = m.Brand?.Name
            }));

        [HttpPost]
        public async Task<IActionResult> CreateModel([FromBody] ModelInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message = "Model adı boş ola bilməz." });
            var model = await _adminService.CreateModelAsync(input.Name, input.ModelCode, input.ReleaseYear, input.CategoryId, input.BrandId);
            return Ok(new { success = true, id = model.Id, message = "Model əlavə edildi." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateModel(int id, [FromBody] ModelInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message = "Model adı boş ola bilməz." });
            var success = await _adminService.UpdateModelAsync(id, input.Name, input.ModelCode, input.ReleaseYear, input.CategoryId, input.BrandId, input.IsActive);
            return success ? Ok(new { success = true, message = "Model yeniləndi." }) : NotFound(new { message = "Model tapılmadı." });
        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> ToggleModel(int id)
        {
            var success = await _adminService.ToggleModelAsync(id);
            return success ? Ok(new { success = true }) : NotFound(new { message = "Model tapılmadı." });
        }

        // ============================================
        // KATALOQ: HİSSƏ NÖVLƏRİ VƏ MATRİS
        // ============================================
        [HttpGet]
        public async Task<IActionResult> PartTypes()
            => Ok((await _adminService.GetPartTypesAsync()).Select(pt => new
            {
                id = pt.Id,
                name = pt.Name,
                key = pt.Key,
                isAccessory = pt.IsAccessory,
                isPortable = pt.IsPortable
            }));

        [HttpPost]
        public async Task<IActionResult> CreatePartType([FromBody] PartTypeInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message = "Hissə növü adı boş ola bilməz." });
            var pt = await _adminService.CreatePartTypeAsync(input.Name, input.Key ?? string.Empty);
            return Ok(new { success = true, id = pt.Id, message = "Hissə növü əlavə edildi." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePartType(int id, [FromBody] PartTypeInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message = "Hissə növü adı boş ola bilməz." });
            var success = await _adminService.UpdatePartTypeAsync(id, input.Name, input.Key ?? string.Empty, input.IsAccessory, input.IsPortable);
            return success ? Ok(new { success = true, message = "Hissə növü yeniləndi." }) : NotFound(new { message = "Hissə növü tapılmadı." });
        }

        [HttpGet]
        public async Task<IActionResult> CategoryPartTypes()
            => Ok((await _adminService.GetCategoryPartTypesAsync()).Select(cp => new { categoryId = cp.CategoryId, partTypeId = cp.PartTypeId }));

        [HttpPost]
        public async Task<IActionResult> AssignPartType([FromBody] AssignPartTypeInput input)
        {
            var success = await _adminService.AssignPartTypeToCategoryAsync(input.CategoryId, input.PartTypeId);
            return success ? Ok(new { success = true }) : BadRequest(new { message = "Əlaqələndirmə baş tutmadı." });
        }

        [HttpPost]
        public async Task<IActionResult> RemovePartType([FromBody] AssignPartTypeInput input)
        {
            var success = await _adminService.RemovePartTypeFromCategoryAsync(input.CategoryId, input.PartTypeId);
            return success ? Ok(new { success = true }) : NotFound(new { message = "Əlaqə tapılmadı." });
        }

        [HttpGet]
        public async Task<IActionResult> QualityTypes()
            => Ok((await _adminService.GetQualityTypesAsync()).Select(q => new
            {
                id = q.Id,
                name = q.Name,
                code = q.Code,
                description = q.Description,
                displayOrder = q.DisplayOrder
            }));

        // ============================================
        // AUDİT VƏ JURNALLAR
        // ============================================
        [HttpGet]
        public async Task<IActionResult> ActivityLogs()
            => Ok((await _adminService.GetActivityLogsAsync(150)).Select(l => new
            {
                id = l.Id,
                userId = l.UserId,
                action = l.Action,
                entityName = l.EntityName,
                entityId = l.EntityId,
                details = l.Details,
                date = l.Date
            }));

        [HttpGet]
        public async Task<IActionResult> SaleAudits()
            => Ok((await _adminService.GetSaleAuditsAsync(150)).Select(s => new
            {
                id = s.Id,
                sparePartId = s.SparePartId,
                title = s.SparePart?.Title,
                sku = s.SparePart?.SKU,
                price = s.SparePart?.Price,
                sellerUserId = s.SellerId,
                quantity = s.Quantity,
                previousStock = s.PreviousStock,
                newStock = s.NewStock,
                timestamp = s.Timestamp
            }));
    }
}
