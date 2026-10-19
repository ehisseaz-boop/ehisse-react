using EHisse.Domain.Entities;
using EHisse.Domain.Enums;
using EHisse.Application.DTOs;
using EHisse.Application.Common;
using EHisse.Persistence.Context;
using EHisse.Persistence.Repositories;
using EHisse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisse.WebApi.Controllers
{
    public record PartInput(
        int DeviceModelId, int PartTypeId, int QualityTypeId,
        string? Title, string? Description, string? SKU,
        decimal Price, decimal? DiscountPrice, string? Currency,
        int StockQuantity, int MinOrderQuantity, int WarrantyMonths,
        double? Latitude = null, double? Longitude = null, string? LocationAddress = null);

    public record PriceInput(decimal NewPrice);

    public record SettingsInput(string CompanyName, string ContactPerson, string? Phone, string? WhatsAppNumber, string? Address, bool ShowContactInfoInAds, double? Latitude = null, double? Longitude = null, string? ShopPhotoUrl = null);

    [Authorize(Roles = "Usta,Anbardar,Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly AppDbContext _context;
        private readonly IBlobStorageService _blobService;

        public WarehouseController(IWarehouseService warehouseService, AppDbContext context, IBlobStorageService blobService)
        {
            _warehouseService = warehouseService;
            _context = context;
            _blobService = blobService;
        }

        private int CurrentUserId
        {
            get
            {
                var val = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(val) && Request.Headers.TryGetValue("X-User-Id", out var hVal))
                    val = hVal.ToString();
                return int.TryParse(val, out int id) ? id : 0;
            }
        }

        private async Task<SellerProfile?> GetSellerAsync()
        {
            var profile = await _warehouseService.GetSellerProfileAsync(CurrentUserId);
            if (profile == null)
            {
                var user = await _context.Users.FindAsync(CurrentUserId);
                if (user != null)
                {
                    profile = new SellerProfile
                    {
                        UserId = user.Id,
                        CompanyName = $"{user.FirstName} {user.LastName}",
                        ContactPerson = $"{user.FirstName} {user.LastName}",
                        Phone = user.Phone,
                        WhatsAppNumber = user.Phone,
                        Rating = 5.0m,
                        ShowContactInfoInAds = true,
                        IsPhoneVerified = true
                    };
                    _context.SellerProfiles.Add(profile);
                    await _context.SaveChangesAsync();
                }
            }
            return profile;
        }

        private static IActionResult NoSeller() => new ObjectResult(new { message = "Satıcı profili tapılmadı." }) { StatusCode = 403 };


        // GET api/Warehouse/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var parts = await _context.SpareParts
                .Include(p => p.DeviceModel).ThenInclude(m => m.Brand)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.SellerId == seller.Id && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            var urlMap = _blobService.GetBulkUrls(parts.SelectMany(p => p.Images).Select(i => i.ImageUrl).Where(u => !string.IsNullOrEmpty(u)).Distinct());

            return Ok(new
            {
                seller = new { 
                    seller.Id, 
                    seller.CompanyName,
                    seller.Address,
                    seller.Latitude,
                    seller.Longitude,
                    seller.ShopPhotoUrl
                },
                parts = parts.Select(p =>
                {
                    var img = p.Images.FirstOrDefault(i => i.IsMain) ?? p.Images.FirstOrDefault();
                    var lastReview = p.Reviews.OrderByDescending(r => r.ReviewDate).FirstOrDefault();
                    var rawPartType = p.PartType?.Name ?? "";
                    var cleanPartType = rawPartType
                        .Replace("Telefon üçün ekranlar", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                        .Replace("Telefon üçün batareyalar", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                        .Replace("Telefon üçün ekran", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                        .Replace("Telefon üçün batareya", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                        .Replace("Telefon üçün ", "", StringComparison.OrdinalIgnoreCase).Trim();

                    var cleanTitle = (p.Title ?? "")
                        .Replace("Telefon üçün ekranlar", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                        .Replace("Telefon üçün batareyalar", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                        .Replace("Telefon üçün ekran", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                        .Replace("Telefon üçün batareya", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                        .Replace("Telefon üçün ", "", StringComparison.OrdinalIgnoreCase).Trim();

                    return new
                    {
                        id = p.Id,
                        title = cleanTitle,
                        sku = p.SKU,
                        price = p.Price,
                        discountPrice = p.DiscountPrice,
                        stockQuantity = p.StockQuantity,
                        viewCount = p.ViewCount,
                        isActive = p.IsActive,
                        moderationStatus = p.ModerationStatus.ToString(),
                        rejectionReason = p.RejectionReason ?? lastReview?.Reason,
                        brandName = p.DeviceModel?.Brand?.Name,
                        modelName = p.DeviceModel?.Name,
                        partTypeName = cleanPartType,
                        qualityTypeName = p.QualityType?.Name,
                        createdDate = p.CreatedDate,
                        image = img == null ? null : (urlMap.TryGetValue(img.ImageUrl, out var u) ? u : img.ImageUrl)
                    };
                })
            });
        }

        // GET api/Warehouse/FormData — kateqoriya və keyfiyyət siyahıları (Add/Edit formu üçün)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FormData()
        {
            var categories = await _warehouseService.GetCategoriesAsync();
            var qualities = await _warehouseService.GetQualityTypesAsync();
            return Ok(new
            {
                categories = categories.Select(c => new { id = c.Id, name = c.Name }),
                qualityTypes = qualities.Select(q => new { id = q.Id, name = q.Name })
            });
        }

        private async Task<List<string>> ValidateAsync(PartInput input, int sellerId, string? currentSku)
        {
            var errors = new List<string>();
            if (input.DeviceModelId <= 0) errors.Add("Zəhmət olmasa cihaz modelini seçin.");
            if (input.PartTypeId <= 0) errors.Add("Hissə növü seçilməlidir.");
            if (input.QualityTypeId <= 0) errors.Add("Keyfiyyət növü seçilməlidir.");
            if (string.IsNullOrWhiteSpace(input.Title) || input.Title.Trim().Length < 3 || input.Title.Length > 200)
                errors.Add("Adın uzunluğu 3 ilə 200 simvol arasında olmalıdır.");
            if (input.Price <= 0) errors.Add("Qiymət sıfırdan böyük olmalıdır.");
            if (input.WarrantyMonths < 0) errors.Add("Zəmanət müddəti mənfi ola bilməz.");
            if (input.StockQuantity < 0) errors.Add("Stok miqdarı mənfi ola bilməz.");
            if (!string.IsNullOrEmpty(input.SKU) && input.SKU != currentSku && await _warehouseService.SkuExistsAsync(input.SKU, sellerId))
                errors.Add("Bu SKU artıq istifadə olunub.");
            return errors;
        }

        private async Task<PartInput> EnsureTitleAsync(PartInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                var model = await _context.DeviceModels.Include(m => m.Brand).FirstOrDefaultAsync(m => m.Id == input.DeviceModelId);
                var partType = await _context.SparePartTypes.FirstOrDefaultAsync(pt => pt.Id == input.PartTypeId);
                var rawPartTypeName = partType?.Name ?? "";
                var cleanPartTypeName = rawPartTypeName
                    .Replace("Telefon üçün ekranlar", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                    .Replace("Telefon üçün batareyalar", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                    .Replace("Telefon üçün ekran", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                    .Replace("Telefon üçün batareya", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                    .Replace("Telefon üçün ", "", StringComparison.OrdinalIgnoreCase).Trim();

                var gen = $"{model?.Brand?.Name} {model?.Name} {cleanPartTypeName}".Trim();
                if (string.IsNullOrWhiteSpace(gen)) gen = "Ehtiyat Hissəsi";
                return input with { Title = gen };
            }
            return input with
            {
                Title = (input.Title ?? "")
                    .Replace("Telefon üçün ekranlar", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                    .Replace("Telefon üçün batareyalar", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                    .Replace("Telefon üçün ekran", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                    .Replace("Telefon üçün batareya", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                    .Replace("Telefon üçün ", "", StringComparison.OrdinalIgnoreCase).Trim()
            };
        }

        private static SparePart ToEntity(PartInput i) => new SparePart
        {
            DeviceModelId = i.DeviceModelId,
            PartTypeId = i.PartTypeId,
            QualityTypeId = i.QualityTypeId,
            Title = (i.Title ?? string.Empty).Trim(),
            Description = i.Description ?? string.Empty,
            SKU = i.SKU ?? string.Empty,
            Price = i.Price,
            DiscountPrice = i.DiscountPrice,
            Currency = string.IsNullOrEmpty(i.Currency) ? "AZN" : i.Currency,
            StockQuantity = i.StockQuantity,
            MinOrderQuantity = i.MinOrderQuantity <= 0 ? 1 : i.MinOrderQuantity,
            WarrantyMonths = i.WarrantyMonths,
            Latitude = i.Latitude,
            Longitude = i.Longitude,
            LocationAddress = i.LocationAddress
        };

        // POST api/Warehouse/AddPart
        [HttpPost]
        public async Task<IActionResult> AddPart([FromBody] PartInput input)
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            input = await EnsureTitleAsync(input);

            var errors = await ValidateAsync(input, seller.Id, null);
            if (errors.Any()) return BadRequest(new { message = string.Join(" ", errors), errors });

            var part = await _warehouseService.AddPartAsync(ToEntity(input), CurrentUserId, seller.Id);
            return Ok(new { success = true, id = part.Id, message = "Elan yaradıldı və təsdiq üçün admin moderasiyasına göndərildi." });
        }

        // GET api/Warehouse/EditPart/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> EditPart(int id)
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var part = await _context.SpareParts
                .Include(p => p.DeviceModel)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == seller.Id && !p.IsDeleted);
            if (part == null) return NotFound(new { message = "Elan tapılmadı." });

            var urlMap = _blobService.GetBulkUrls(part.Images.Select(i => i.ImageUrl).Where(u => !string.IsNullOrEmpty(u)));

            return Ok(new
            {
                id = part.Id,
                categoryId = part.DeviceModel?.CategoryId,
                brandId = part.DeviceModel?.BrandId,
                deviceModelId = part.DeviceModelId,
                partTypeId = part.PartTypeId,
                qualityTypeId = part.QualityTypeId,
                title = part.Title,
                description = part.Description,
                sku = part.SKU,
                price = part.Price,
                discountPrice = part.DiscountPrice,
                currency = part.Currency,
                stockQuantity = part.StockQuantity,
                minOrderQuantity = part.MinOrderQuantity,
                warrantyMonths = part.WarrantyMonths,
                latitude = part.Latitude,
                longitude = part.Longitude,
                locationAddress = part.LocationAddress,
                isActive = part.IsActive,
                moderationStatus = part.ModerationStatus.ToString(),
                images = part.Images.OrderBy(i => i.DisplayOrder).Select(i => new
                {
                    id = i.Id,
                    isMain = i.IsMain,
                    url = urlMap.TryGetValue(i.ImageUrl, out var u) ? u : i.ImageUrl
                })
            });
        }

        // PUT api/Warehouse/EditPart/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditPart(int id, [FromBody] PartInput input)
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var part = await _warehouseService.GetPartAsync(id, seller.Id);
            if (part == null) return NotFound(new { message = "Elan tapılmadı." });

            input = await EnsureTitleAsync(input);

            var errors = await ValidateAsync(input, seller.Id, part.SKU);
            if (errors.Any()) return BadRequest(new { message = string.Join(" ", errors), errors });

            var result = await _warehouseService.EditPartAsync(id, ToEntity(input), CurrentUserId, seller.Id);
            if (result.ConcurrencyError)
                return Conflict(new { message = "Eyni vaxtda başqa istifadəçi tərəfindən dəyişiklik edilib. Səhifəni yeniləyin və yenidən cəhd edin." });
            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            var updatedPart = await _warehouseService.GetPartAsync(id, seller.Id);
            var isPending = updatedPart?.ModerationStatus == ApprovalStatus.Pending;
            var msg = isPending
                ? "Dəyişikliklər yadda saxlanıldı və admin təsdiqinə göndərildi (təsdiqləndikdən sonra saytda aktivləşəcək)."
                : "Dəyişikliklər uğurla yadda saxlanıldı.";

            return Ok(new { success = true, message = msg, moderationStatus = updatedPart?.ModerationStatus.ToString(), isActive = updatedPart?.IsActive });
        }

        // POST api/Warehouse/UpdatePrice/5
        [HttpPost("{id:int}")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] PriceInput input)
        {
            if (input.NewPrice <= 0) return BadRequest(new { message = "Mənfi və ya sıfır qiymət qəbul edilmir." });

            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var success = await _warehouseService.UpdatePriceAsync(id, input.NewPrice, seller.Id);
            if (!success) return NotFound(new { message = "Elan tapılmadı." });

            return Ok(new { success = true });
        }

        // POST api/Warehouse/ToggleActive/5
        [HttpPost("{id:int}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var result = await _warehouseService.ToggleActiveAsync(id, seller.Id);
            if (!result.Success && result.ErrorMessage == "Not Found") return NotFound(new { message = "Elan tapılmadı." });
            if (!result.Success) return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { success = true });
        }

        // DELETE api/Warehouse/DeletePart/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var success = await _warehouseService.DeletePartAsync(id, seller.Id);
            if (!success) return NotFound(new { message = "Elan tapılmadı." });

            return Ok(new { success = true, message = "Elan zibil qutusuna göndərildi." });
        }

        // GET api/Warehouse/DeletedAds
        [HttpGet]
        public async Task<IActionResult> DeletedAds()
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var parts = await _context.SpareParts
                .Include(p => p.DeviceModel).ThenInclude(m => m.Brand)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Images)
                .Where(p => p.SellerId == seller.Id && p.IsDeleted)
                .OrderByDescending(p => p.LastModifiedDate)
                .ToListAsync();

            var urlMap = _blobService.GetBulkUrls(parts.SelectMany(p => p.Images).Select(i => i.ImageUrl).Where(u => !string.IsNullOrEmpty(u)).Distinct());

            return Ok(new
            {
                seller = new { seller.Id, seller.CompanyName },
                parts = parts.Select(p =>
                {
                    var img = p.Images.FirstOrDefault(i => i.IsMain) ?? p.Images.FirstOrDefault();
                    return new
                    {
                        id = p.Id,
                        title = p.Title,
                        sku = p.SKU,
                        price = p.Price,
                        stockQuantity = p.StockQuantity,
                        brandName = p.DeviceModel?.Brand?.Name,
                        modelName = p.DeviceModel?.Name,
                        partTypeName = p.PartType?.Name,
                        qualityTypeName = p.QualityType?.Name,
                        deletedDate = p.LastModifiedDate,
                        image = img == null ? null : (urlMap.TryGetValue(img.ImageUrl, out var u) ? u : img.ImageUrl)
                    };
                })
            });
        }

        // POST api/Warehouse/RestorePart/5
        [HttpPost("{id:int}")]
        public async Task<IActionResult> RestorePart(int id)
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == seller.Id && p.IsDeleted);
            if (part == null) return NotFound(new { message = "Silinmiş elan tapılmadı." });

            part.IsDeleted = false;
            part.IsActive = true;
            part.LastModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Elan uğurla bərpa edildi." });
        }

        // POST api/Warehouse/RecordSale/5
        [HttpPost("{id:int}")]
        public async Task<IActionResult> RecordSale(int id)
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var result = await _warehouseService.RecordSaleAsync(id, CurrentUserId, seller.Id);
            if (!result.Success)
            {
                if (result.ConcurrencyError)
                    return Conflict(new { message = "Eyni vaxtda başqa istifadəçi tərəfindən satış edilib. Səhifəni yeniləyin və yenidən cəhd edin." });
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { success = true });
        }

        // GET api/Warehouse/SalesHistory
        [HttpGet]
        public async Task<IActionResult> SalesHistory()
        {
            var seller = await GetSellerAsync();
            if (seller == null) return NoSeller();

            var sales = await _context.SparePartSaleAudits
                .Include(s => s.SparePart)
                .Where(s => s.SellerId == seller.Id)
                .OrderByDescending(s => s.Timestamp)
                .Take(200)
                .Select(s => new
                {
                    id = s.Id,
                    sparePartId = s.SparePartId,
                    title = s.SparePart.Title,
                    sku = s.SparePart.SKU,
                    price = s.SparePart.Price,
                    quantity = s.Quantity,
                    previousStock = s.PreviousStock,
                    newStock = s.NewStock,
                    timestamp = s.Timestamp
                })
                .ToListAsync();

            return Ok(sales);
        }

        // GET api/Warehouse/Settings
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _warehouseService.GetUserWithSellerProfileAsync(CurrentUserId);
            if (user == null || user.SellerProfile == null) return NotFound(new { message = "Satıcı profili tapılmadı." });

            var s = user.SellerProfile;
            return Ok(new
            {
                firstName = user.FirstName,
                lastName = user.LastName,
                fullName = $"{user.FirstName} {user.LastName}".Trim(),
                pin = user.Pin,
                idCardImageUrl = user.IdCardImageUrl,
                email = user.Email,
                companyName = s.CompanyName,
                contactPerson = s.ContactPerson,
                phone = s.Phone,
                whatsAppNumber = s.WhatsAppNumber,
                address = s.Address,
                latitude = s.Latitude,
                longitude = s.Longitude,
                shopPhotoUrl = s.ShopPhotoUrl,
                showContactInfoInAds = true,
                rating = s.Rating
            });
        }

        // PUT api/Warehouse/Settings
        [HttpPut]
        public async Task<IActionResult> Settings([FromBody] SettingsInput input)
        {
            var updated = new SellerProfile
            {
                CompanyName = input.CompanyName,
                ContactPerson = input.ContactPerson,
                Phone = input.Phone,
                WhatsAppNumber = input.WhatsAppNumber,
                Address = input.Address,
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                ShopPhotoUrl = input.ShopPhotoUrl,
                ShowContactInfoInAds = true
            };

            var result = await _warehouseService.UpdateSettingsAsync(CurrentUserId, updated);
            if (!result.Success && result.ErrorMessage == "Not Found") return NotFound(new { message = "Satıcı profili tapılmadı." });
            if (!result.Success) return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { success = true, message = "Tənzimləmələr uğurla yadda saxlanıldı." });
        }
    }
}
