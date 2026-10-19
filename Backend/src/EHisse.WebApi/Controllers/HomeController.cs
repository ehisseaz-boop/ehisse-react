using EHisse.Persistence.Repositories;
using EHisse.Domain.Entities;
using EHisse.Domain.Enums;
using EHisse.Application.DTOs;
using EHisse.Application.Common;
using EHisse.Persistence.Context;
using EHisse.Persistence.Repositories;
using EHisse.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisse.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBlobStorageService _blobService;
        private readonly ICacheService _cacheService;

        public HomeController(AppDbContext context, IBlobStorageService blobService, ICacheService cacheService)
        {
            _context = context;
            _blobService = blobService;
            _cacheService = cacheService;
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

            bool isDefaultFirstPage = string.IsNullOrWhiteSpace(searchString)
                && !categoryId.HasValue
                && !brandId.HasValue
                && !modelId.HasValue
                && !partTypeId.HasValue
                && !qualityTypeId.HasValue
                && !minPrice.HasValue
                && !maxPrice.HasValue
                && !sellerId.HasValue
                && inStockOnly
                && (string.IsNullOrEmpty(sortOrder) || sortOrder == "date_desc")
                && pageNumber == 1
                && pageSize == 20;

            if (isDefaultFirstPage)
            {
                var cachedIndex = await _cacheService.GetOrCreateAsync<object>("home_index_default", async () =>
                {
                    return await ExecuteIndexAsync(
                        searchString, categoryId, brandId, modelId, partTypeId, qualityTypeId,
                        minPrice, maxPrice, sellerId, inStockOnly, sortOrder, pageNumber, pageSize);
                }, absoluteExpiration: TimeSpan.FromMinutes(2), tag: "home_data");

                return Ok(cachedIndex);
            }

            var result = await ExecuteIndexAsync(
                searchString, categoryId, brandId, modelId, partTypeId, qualityTypeId,
                minPrice, maxPrice, sellerId, inStockOnly, sortOrder, pageNumber, pageSize);

            return Ok(result);
        }

        private async Task<object> ExecuteIndexAsync(
            string? searchString,
            int? categoryId,
            int? brandId,
            int? modelId,
            int? partTypeId,
            int? qualityTypeId,
            decimal? minPrice,
            decimal? maxPrice,
            int? sellerId,
            bool inStockOnly,
            string sortOrder,
            int pageNumber,
            int pageSize)
        {
            // Sidebar üçün ümumi statistika (filtrsiz) - cached for 3 mins
            var stats = await _cacheService.GetOrCreateAsync($"home_stats_{inStockOnly}", async () =>
            {
                var activeAdsQuery = _context.SpareParts
                    .AsNoTracking()
                    .Where(p => p.ModerationStatus == ApprovalStatus.Approved && !p.IsDeleted && p.IsActive);
                if (inStockOnly) activeAdsQuery = activeAdsQuery.Where(p => p.StockQuantity > 0);

                var categoryCounts = await activeAdsQuery
                    .GroupBy(p => p.DeviceModel.CategoryId)
                    .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(k => k.CategoryId, v => v.Count);

                var totalParts = await activeAdsQuery.CountAsync();
                var totalSellers = await _context.SellerProfiles.AsNoTracking().CountAsync();

                return new HomeStatsDto(categoryCounts, totalParts, totalSellers);
            }, absoluteExpiration: TimeSpan.FromMinutes(3), tag: "home_data");

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
                .AsNoTracking()
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

            return new
            {
                items = parts.Select(p => MapPart(p, urlMap)).ToList(),
                pageNumber,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                totalItems,
                totalParts = stats.TotalParts,
                totalSellers = stats.TotalSellers,
                categoryCounts = stats.CategoryCounts
            };
        }

        // GET api/Home/Saticilar
        [HttpGet]
        public async Task<IActionResult> Saticilar(
            string? searchString,
            string? role,
            string? searchType)
        {
            var query = _context.SellerProfiles
                .AsNoTracking()
                .Include(s => s.User)
                .Where(s => !s.User.IsBlocked && s.User.ApprovalStatus == ApprovalStatus.Approved);

            // Növ / Rol üzrə filtr (usta, anbardar, servis)
            if (!string.IsNullOrWhiteSpace(role) && !role.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                if (role.Equals("usta", StringComparison.OrdinalIgnoreCase) || role == "2")
                {
                    query = query.Where(s => s.User.Role == UserRole.Usta);
                }
                else if (role.Equals("anbardar", StringComparison.OrdinalIgnoreCase) || role == "3")
                {
                    query = query.Where(s => s.User.Role == UserRole.Anbardar);
                }
                else if (role.Equals("servis", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(s => s.CompanyName.ToLower().Contains("servis") ||
                                             s.CompanyName.ToLower().Contains("service") ||
                                             (s.User.Role != UserRole.Usta && s.User.Role != UserRole.Anbardar));
                }
            }

            // Axtarış: Servisin adına, ustanın adına və anbardarın adına görə
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim();
                var lowerTerm = $"%{term.ToLower()}%";

                if (string.Equals(searchType, "servis", StringComparison.OrdinalIgnoreCase))
                {
                    // Yalnız servisin adına görə
                    query = query.Where(s => EF.Functions.Like(s.CompanyName.ToLower(), lowerTerm));
                }
                else if (string.Equals(searchType, "usta", StringComparison.OrdinalIgnoreCase))
                {
                    // Yalnız ustanın adına görə
                    query = query.Where(s =>
                        (s.User.Role == UserRole.Usta || s.CompanyName.ToLower().Contains("usta")) &&
                        (EF.Functions.Like(s.ContactPerson.ToLower(), lowerTerm) ||
                         EF.Functions.Like(s.User.FirstName.ToLower(), lowerTerm) ||
                         EF.Functions.Like(s.User.LastName.ToLower(), lowerTerm) ||
                         EF.Functions.Like((s.User.FirstName + " " + s.User.LastName).ToLower(), lowerTerm) ||
                         EF.Functions.Like(s.User.Username.ToLower(), lowerTerm)));
                }
                else if (string.Equals(searchType, "anbardar", StringComparison.OrdinalIgnoreCase))
                {
                    // Yalnız anbardarın adına görə
                    query = query.Where(s =>
                        (s.User.Role == UserRole.Anbardar || s.CompanyName.ToLower().Contains("anbar")) &&
                        (EF.Functions.Like(s.ContactPerson.ToLower(), lowerTerm) ||
                         EF.Functions.Like(s.User.FirstName.ToLower(), lowerTerm) ||
                         EF.Functions.Like(s.User.LastName.ToLower(), lowerTerm) ||
                         EF.Functions.Like((s.User.FirstName + " " + s.User.LastName).ToLower(), lowerTerm) ||
                         EF.Functions.Like(s.User.Username.ToLower(), lowerTerm)));
                }
                else
                {
                    // Hamısı üzrə axtarış: servisin adına, ustanın adına, anbardarın adına və ünvanına görə
                    bool searchIsUsta = term.Equals("usta", StringComparison.OrdinalIgnoreCase);
                    bool searchIsAnbardar = term.Equals("anbardar", StringComparison.OrdinalIgnoreCase) || term.Equals("anbar", StringComparison.OrdinalIgnoreCase);
                    bool searchIsServis = term.Equals("servis", StringComparison.OrdinalIgnoreCase) || term.Equals("service", StringComparison.OrdinalIgnoreCase);

                    query = query.Where(s =>
                        EF.Functions.Like(s.CompanyName.ToLower(), lowerTerm) ||
                        EF.Functions.Like(s.ContactPerson.ToLower(), lowerTerm) ||
                        EF.Functions.Like(s.User.FirstName.ToLower(), lowerTerm) ||
                        EF.Functions.Like(s.User.LastName.ToLower(), lowerTerm) ||
                        EF.Functions.Like((s.User.FirstName + " " + s.User.LastName).ToLower(), lowerTerm) ||
                        EF.Functions.Like(s.User.Username.ToLower(), lowerTerm) ||
                        (searchIsUsta && s.User.Role == UserRole.Usta) ||
                        (searchIsAnbardar && s.User.Role == UserRole.Anbardar) ||
                        (searchIsServis && s.CompanyName.ToLower().Contains("servis")) ||
                        (s.Address != null && EF.Functions.Like(s.Address.ToLower(), lowerTerm)));
                }
            }

            var sellers = await query.ToListAsync();
            var sellerIds = sellers.Select(s => s.Id).ToList();

            // Hər satıcı üçün elan statistikası:
            // - Total: İndiyədək yerləşdirdiyi bütün elanların sayı
            // - Active: Hazırda aktiv, təsdiqlənmiş və stokda olan elan sayı
            var adStats = await _context.SpareParts
                .AsNoTracking()
                .Where(p => sellerIds.Contains(p.SellerId))
                .GroupBy(p => p.SellerId)
                .Select(g => new
                {
                    SellerId = g.Key,
                    Total = g.Count(),
                    Active = g.Count(p => !p.IsDeleted && p.IsActive && p.ModerationStatus == ApprovalStatus.Approved && p.StockQuantity > 0)
                })
                .ToDictionaryAsync(k => k.SellerId);

            return Ok(sellers.Select(s =>
            {
                var role = s.User?.Role;
                string roleName = role switch
                {
                    UserRole.Usta => "Usta",
                    UserRole.Anbardar => "Anbardar",
                    _ => s.CompanyName.ToLower().Contains("servis") ? "Servis" : "Satıcı / Mağaza"
                };

                return new
                {
                    id = s.Id,
                    companyName = s.CompanyName,
                    contactPerson = s.ContactPerson,
                    firstName = s.User?.FirstName,
                    lastName = s.User?.LastName,
                    userRole = role?.ToString(),
                    userRoleId = (int?)role,
                    roleName = roleName,
                    phone = s.ShowContactInfoInAds ? s.Phone : null,
                    whatsAppNumber = s.ShowContactInfoInAds ? s.WhatsAppNumber : null,
                    address = s.Address,
                    rating = s.Rating,
                    isPhoneVerified = s.IsPhoneVerified,
                    totalAds = adStats.TryGetValue(s.Id, out var st) ? st.Total : 0,
                    activeAds = adStats.TryGetValue(s.Id, out var st2) ? st2.Active : 0
                };
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
            var cached = await _cacheService.GetOrCreateAsync("home_categories", async () =>
            {
                var counts = await _context.SpareParts
                    .AsNoTracking()
                    .Where(p => p.ModerationStatus == ApprovalStatus.Approved && !p.IsDeleted && p.IsActive && p.StockQuantity > 0)
                    .GroupBy(p => p.DeviceModel.CategoryId)
                    .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(k => k.CategoryId, v => v.Count);

                var categories = await _context.Categories
                    .AsNoTracking()
                    .OrderBy(c => c.DisplayOrder)
                    .Select(c => new
                    {
                        id = c.Id,
                        name = c.Name,
                        displayOrder = c.DisplayOrder,
                        isActive = c.IsActive
                    })
                    .ToListAsync();

                return categories.Select(c => new
                {
                    c.id,
                    c.name,
                    c.displayOrder,
                    c.isActive,
                    count = counts.TryGetValue(c.id, out var n) ? n : 0
                }).ToList();
            }, slidingExpiration: TimeSpan.FromMinutes(45), tag: "categories");

            return Ok(cached);
        }

        // GET api/Home/Details/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var part = await _context.SpareParts
                .AsNoTracking()
                .Include(p => p.DeviceModel).ThenInclude(m => m.Brand)
                .Include(p => p.DeviceModel).ThenInclude(m => m.Category)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (part == null)
                return NotFound(new { message = "Məhsul tapılmadı." });

            // Atomic view count update without change tracking overhead
            await _context.SpareParts
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));
            part.ViewCount++;

            var urlMap = _blobService.GetBulkUrls(part.Images.Select(i => i.ImageUrl).Where(u => !string.IsNullOrEmpty(u)));

            // Cari istifadəçi bu elanın sahibidirmi?
            bool isOwner = false;
            if (User.Identity?.IsAuthenticated == true &&
                int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                isOwner = await _context.SellerProfiles.AsNoTracking().AnyAsync(s => s.UserId == userId && s.Id == part.SellerId);
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

        private static string CleanPartTypeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            return name
                .Replace("Telefon üçün ekranlar", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                .Replace("Telefon üçün batareyalar", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                .Replace("Telefon üçün ekran", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                .Replace("Telefon üçün batareya", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                .Replace("Telefon üçün ", "", StringComparison.OrdinalIgnoreCase).Trim();
        }

        private static string CleanTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title)) return "";
            return title
                .Replace("Telefon üçün ekranlar", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                .Replace("Telefon üçün batareyalar", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                .Replace("Telefon üçün ekran", "Ekranlar", StringComparison.OrdinalIgnoreCase)
                .Replace("Telefon üçün batareya", "Batareyalar", StringComparison.OrdinalIgnoreCase)
                .Replace("Telefon üçün ", "", StringComparison.OrdinalIgnoreCase).Trim();
        }

        private static object MapPart(SparePart p, IDictionary<string, string> urlMap) => new
        {
            id = p.Id,
            title = CleanTitle(p.Title),
            price = p.Price,
            stockQuantity = p.StockQuantity,
            viewCount = p.ViewCount,
            createdDate = p.CreatedDate,
            brandName = p.DeviceModel?.Brand?.Name,
            modelName = p.DeviceModel?.Name,
            partTypeName = CleanPartTypeName(p.PartType?.Name),
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
