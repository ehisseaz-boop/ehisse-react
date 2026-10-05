using EHisseler.Web.Models;
using EHisseler.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisseler.Web.Controllers
{
    public record ReasonInput(string? Reason);

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
            email = u.Email,
            phone = u.Phone,
            role = u.Role.ToString(),
            approvalStatus = u.ApprovalStatus.ToString(),
            isBlocked = u.IsBlocked,
            createdDate = u.CreatedDate
        };

        private static object MapAd(SparePart p) => new
        {
            id = p.Id,
            title = p.Title,
            sku = p.SKU,
            price = p.Price,
            stockQuantity = p.StockQuantity,
            isActive = p.IsActive,
            isDeleted = p.IsDeleted,
            moderationStatus = p.ModerationStatus.ToString(),
            rejectionReason = p.RejectionReason,
            isUpdateSubmission = p.IsUpdateSubmission,
            brandName = p.DeviceModel?.Brand?.Name,
            modelName = p.DeviceModel?.Name,
            partTypeName = p.PartType?.Name,
            sellerName = p.Seller?.CompanyName ?? p.Seller?.ContactPerson,
            createdDate = p.CreatedDate
        };

        // GET api/Admin/Requests — gözləyən istifadəçilər + elanlar
        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            var pendingUsers = await _adminService.GetPendingUsersAsync();
            var pendingAds = await _adminService.GetPendingAdsAsync();
            return Ok(new
            {
                pendingUsers = pendingUsers.Select(MapUser),
                newAdRequests = pendingAds.Where(p => !p.IsUpdateSubmission).Select(MapAd),
                updateAdRequests = pendingAds.Where(p => p.IsUpdateSubmission).Select(MapAd)
            });
        }

        // GET api/Admin/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pendingUsers = await _adminService.GetPendingUsersAsync();
            return Ok(pendingUsers.Select(MapUser));
        }

        [HttpPost("{userId:int}")]
        public async Task<IActionResult> ApproveUser(int userId)
            => await _adminService.ApproveUserAsync(userId, AdminId) ? Ok(new { success = true }) : NotFound(new { message = "İstifadəçi tapılmadı." });

        [HttpPost("{userId:int}")]
        public async Task<IActionResult> RejectUser(int userId, [FromBody] ReasonInput input)
            => await _adminService.RejectUserAsync(userId, input.Reason ?? string.Empty, AdminId) ? Ok(new { success = true }) : NotFound(new { message = "İstifadəçi tapılmadı." });

        [HttpPost("{userId:int}")]
        public async Task<IActionResult> BlockUser(int userId, [FromBody] ReasonInput input)
            => await _adminService.BlockUserAsync(userId, input.Reason ?? string.Empty, AdminId) ? Ok(new { success = true }) : NotFound(new { message = "İstifadəçi tapılmadı." });

        [HttpPost("{userId:int}")]
        public async Task<IActionResult> UnblockUser(int userId)
            => await _adminService.UnblockUserAsync(userId, AdminId) ? Ok(new { success = true }) : NotFound(new { message = "İstifadəçi tapılmadı." });

        // GET api/Admin/PendingAds
        [HttpGet]
        public async Task<IActionResult> PendingAds()
            => Ok((await _adminService.GetPendingAdsAsync()).Select(MapAd));

        [HttpPost("{adId:int}")]
        public async Task<IActionResult> ApproveAd(int adId)
            => await _adminService.ApproveAdAsync(adId, AdminId) ? Ok(new { success = true }) : NotFound(new { message = "Elan tapılmadı." });

        [HttpPost("{adId:int}")]
        public async Task<IActionResult> RejectAd(int adId, [FromBody] ReasonInput input)
            => await _adminService.RejectAdAsync(adId, input.Reason ?? string.Empty, AdminId) ? Ok(new { success = true }) : NotFound(new { message = "Elan tapılmadı." });

        [HttpPost("{adId:int}")]
        public async Task<IActionResult> RestoreAd(int adId)
        {
            var result = await _adminService.RestoreAdAsync(adId, AdminId);
            if (!result.Success && result.ErrorMessage == "Not Found") return NotFound(new { message = "Elan tapılmadı." });
            if (!result.Success) return BadRequest(new { message = result.ErrorMessage });
            return Ok(new { success = true, message = "Elan bərpa edildi və yoxlamaya göndərildi." });
        }

        [HttpGet]
        public async Task<IActionResult> AllUsers()
            => Ok((await _adminService.GetAllUsersAsync()).Select(MapUser));

        [HttpGet]
        public async Task<IActionResult> AllAds()
            => Ok((await _adminService.GetAllAdsAsync()).Select(MapAd));

        [HttpGet]
        public async Task<IActionResult> Categories()
            => Ok((await _adminService.GetCategoriesAsync()).Select(c => new { id = c.Id, name = c.Name, displayOrder = c.DisplayOrder, isActive = c.IsActive }));

        [HttpGet]
        public async Task<IActionResult> ModelsAndBrands()
            => Ok((await _adminService.GetModelsAndBrandsAsync()).Select(m => new
            {
                id = m.Id,
                name = m.Name,
                modelCode = m.ModelCode,
                releaseYear = m.ReleaseYear,
                isActive = m.IsActive,
                brandName = m.Brand?.Name,
                categoryName = m.Category?.Name
            }));

        [HttpGet]
        public async Task<IActionResult> ActivityLogs()
            => Ok((await _adminService.GetActivityLogsAsync(100)).Select(l => new
            {
                id = l.Id,
                userId = l.UserId,
                action = l.Action,
                entityName = l.EntityName,
                entityId = l.EntityId,
                details = l.Details,
                date = l.Date
            }));
    }
}
