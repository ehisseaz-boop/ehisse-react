using EHisseler.Web.Extensions;
using EHisseler.Web.Models;
using EHisseler.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisseler.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBlobStorageService _blobService;

        public HomeController(AppDbContext context, IBlobStorageService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET api/Home/Index
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchString,
            int? categoryId,
            int? brandId,
            int? modelId,
            int? partTypeId,
            int? qualityTypeId,
            decimal? minPrice,
            decimal? maxPrice,
            int? sellerId,
            bool inStockOnly = true,
            string sortOrder = "",
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (pageNumber < 1) pageNumber = 1;

            // Sidebar üçün ümumi statistika (filtrsiz)
            var activeAdsQuery = _context.SpareParts
                .Where(p => p.ModerationStatus == ApprovalStatus.Approved && !p.IsDeleted && p.IsActive);
            if (inStockOnly) activeAdsQuery = activeAdsQuery.Where(p => p.StockQuantity > 0);

            var categoryCounts = await activeAdsQuery
                .GroupBy(p => p.DeviceModel.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.CategoryId, v => v.Count);

            var totalParts = await activeAdsQuery.CountAsync();
            var totalSellers = await _context.SellerProfiles.CountAsync();

            // ── QueryBuilder ilə dynamic search ────────────────────────────────
            var filter = new SparePartSearchQuery
            {
                SearchString   = searchString,
                CategoryId     = categoryId,
                BrandId        = brandId,
                ModelId        = modelId,
                PartTypeId     = partTypeId,
                QualityTypeId  = qualityTypeId,
                MinPrice       = minPrice,
                MaxPrice       = maxPrice,
                SellerId       = sellerId,
                InStockOnly    = inStockOnly,
                SortOrder      = string.IsNullOrEmpty(sortOrder) ? "date_desc" : sortOrder,
                Status         = ApprovalStatus.Approved,
                IncludeDeleted = false,
                PageNumber     = pageNumber,
                PageSize       = pageSize
            };

            var partsQuery = _context.SpareParts
                .Include(p => p.DeviceModel).ThenInclude(m => m.Brand)
                .Include(p => p.DeviceModel).ThenInclude(m => m.Category)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .ApplySearch(filter);           // ← QueryBuilder: bütün filtrləri tətbiq edir
            // ────────────────────────────────────────────────────────────────────

            var totalItems = await partsQuery.CountAsync();
            var parts = await partsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ── Bulk image URL yükləmə ──────────────────────────────────────────
            var urlMap = _blobService.GetBulkUrls(parts
                .SelectMany(p => p.Images.Select(i => i.ImageUrl))
                .Where(url => !string.IsNullOrEmpty(url))
                .Distinct());

            return Ok(new
            {
                items = parts.Select(p => MapPart(p, urlMap)),
                pageNumber,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                totalItems,
                totalParts,
                totalSellers,
                categoryCounts
            });
        }

        // GET api/Home/Saticilar
        [HttpGet]
        public async Task<IActionResult> Saticilar(string? searchString)
        {
            var query = _context.SellerProfiles
                .Include(s => s.User)
                .Where(s => !s.User.IsBlocked);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s =>
                    s.CompanyName.Contains(searchString) ||
                    s.ContactPerson.Contains(searchString));
            }

            var sellers = await query.ToListAsync();
            var sellerIds = sellers.Select(s => s.Id).ToList();

            var adStats = await _context.SpareParts
                .Where(p => sellerIds.Contains(p.SellerId) && !p.IsDeleted)
                .GroupBy(p => p.SellerId)
                .Select(g => new
                {
                    SellerId = g.Key,
                    Total = g.Count(),
                    Active = g.Count(p => p.ModerationStatus == ApprovalStatus.Approved && p.IsActive)
                })
                .ToDictionaryAsync(k => k.SellerId);

            return Ok(sellers.Select(s => new
            {
                id = s.Id,
                companyName = s.CompanyName,
                contactPerson = s.ContactPerson,
                phone = s.ShowContactInfoInAds ? s.Phone : null,
                whatsAppNumber = s.ShowContactInfoInAds ? s.WhatsAppNumber : null,
                address = s.Address,
                rating = s.Rating,
                totalAds = adStats.TryGetValue(s.Id, out var st) ? st.Total : 0,
                activeAds = adStats.TryGetValue(s.Id, out var st2) ? st2.Active : 0
            }));
        }

        // GET api/Home/Yardim
        [HttpGet]
        public IActionResult Yardim()
        {
            return Ok(new
            {
                title = "Yardım Mərkəzi",
                subtitle = "Sizə necə kömək edə bilərik?",
                topics = new[]
                {
                    new { icon = "bi-truck", title = "Sifariş və Çatdırılma", description = "Sifarişlərin izlənməsi, çatdırılma müddəti və kuryer xidmətləri haqqında məlumatlar." },
                    new { icon = "bi-shield-check", title = "Zəmanət və Qaytarma", description = "Məhsulların zəmanət şərtləri, defektli məhsulların dəyişdirilməsi və geri qaytarma qaydaları." },
                    new { icon = "bi-person-gear", title = "Hesab və Tənzimləmələr", description = "Profil məlumatlarının yenilənməsi, şifrə bərpası və mağaza (satıcı) hesabı açmaq qaydaları." }
                },
                faqs = new[]
                {
                    new { question = "Hissəni necə sifariş edə bilərəm?", answer = "Məhsul səhifəsində satıcının telefon və ya WhatsApp nömrəsi ilə birbaşa əlaqə saxlayın." },
                    new { question = "Satıcı kimi necə qeydiyyatdan keçə bilərəm?", answer = "Qeydiyyat səhifəsində formu doldurun. Admin təsdiqindən sonra elan yerləşdirə bilərsiniz." },
                    new { question = "Elanım nə vaxt yayımlanır?", answer = "Hər elan moderasiyadan keçir və təsdiqləndikdən sonra kataloqda görünür." },
                    new { question = "Şifrəmi unutmuşam, nə etməliyəm?", answer = "Giriş səhifəsində 'Şifrəni unutdum' bölməsindən istifadə edin." }
                }
            });
        }

        // GET api/Home/Elaqe
        [HttpGet]
        public IActionResult Elaqe()
        {
            return Ok(new
            {
                title = "Bizimlə Əlaqə",
                subtitle = "Hər hansı bir sualınız və ya təklifiniz varsa, bizə müraciət edə bilərsiniz.",
                phone = "+994 10 252 32 26",
                whatsApp = "+994 10 396 15 01",
                email = "info@e-hisse.az",
                address = "Bakı ş., Azərbaycan"
            });
        }

        // GET api/Home/Categories
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var counts = await _context.SpareParts
                .Where(p => p.ModerationStatus == ApprovalStatus.Approved && !p.IsDeleted && p.IsActive && p.StockQuantity > 0)
                .GroupBy(p => p.DeviceModel.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.CategoryId, v => v.Count);

            var categories = await _context.Categories.OrderBy(c => c.DisplayOrder).ToListAsync();
            return Ok(categories.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                displayOrder = c.DisplayOrder,
                isActive = c.IsActive,
                count = counts.TryGetValue(c.Id, out var n) ? n : 0
            }));
        }

        // GET api/Home/Details/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var part = await _context.SpareParts
                .Include(p => p.DeviceModel).ThenInclude(m => m.Brand)
                .Include(p => p.DeviceModel).ThenInclude(m => m.Category)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (part == null)
                return NotFound(new { message = "Məhsul tapılmadı." });

            part.ViewCount++;
            await _context.SaveChangesAsync();

            var urlMap = _blobService.GetBulkUrls(part.Images.Select(i => i.ImageUrl).Where(u => !string.IsNullOrEmpty(u)));

            // Cari istifadəçi bu elanın sahibidirmi?
            bool isOwner = false;
            if (User.Identity?.IsAuthenticated == true &&
                int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                isOwner = await _context.SellerProfiles.AnyAsync(s => s.UserId == userId && s.Id == part.SellerId);
            }

            var seller = part.Seller;
            return Ok(new
            {
                part = MapPart(part, urlMap),
                description = part.Description,
                sku = part.SKU,
                currency = part.Currency,
                warrantyMonths = part.WarrantyMonths,
                discountPrice = part.DiscountPrice,
                categoryId = part.DeviceModel?.CategoryId,
                categoryName = part.DeviceModel?.Category?.Name,
                seller = seller == null ? null : new
                {
                    id = seller.Id,
                    companyName = seller.CompanyName,
                    contactPerson = seller.ContactPerson,
                    phone = seller.ShowContactInfoInAds ? seller.Phone : null,
                    whatsAppNumber = seller.ShowContactInfoInAds ? seller.WhatsAppNumber : null,
                    address = seller.Address,
                    rating = seller.Rating
                },
                isOwner
            });
        }

        private static object MapPart(SparePart p, IDictionary<string, string> urlMap) => new
        {
            id = p.Id,
            title = p.Title,
            price = p.Price,
            stockQuantity = p.StockQuantity,
            viewCount = p.ViewCount,
            createdDate = p.CreatedDate,
            brandName = p.DeviceModel?.Brand?.Name,
            modelName = p.DeviceModel?.Name,
            partTypeName = p.PartType?.Name,
            qualityTypeName = p.QualityType?.Name,
            sellerId = p.SellerId,
            sellerName = p.Seller?.CompanyName ?? p.Seller?.ContactPerson,
            latitude = p.Latitude,
            longitude = p.Longitude,
            locationAddress = p.LocationAddress,
            images = p.Images.OrderBy(i => i.DisplayOrder).Select(i => new
            {
                isMain = i.IsMain,
                url = urlMap.TryGetValue(i.ImageUrl, out var url) ? url : i.ImageUrl
            }).ToList()
        };
    }
}
