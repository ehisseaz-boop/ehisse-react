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
using System.Linq;
using System.Threading.Tasks;

namespace EHisse.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AjaxController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBlobStorageService _blobService;
        private readonly ICacheService _cacheService;

        public AjaxController(AppDbContext context, IBlobStorageService blobService, ICacheService cacheService)
        {
            _context = context;
            _blobService = blobService;
            _cacheService = cacheService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetBrandsByCategory(int categoryId)
        {
            var realBrands = await _cacheService.GetOrCreateAsync($"ajax_brands_{categoryId}", async () =>
            {
                return await _context.DeviceBrands
                    .AsNoTracking()
                    .Where(b => _context.CategoryBrands.Any(cb => cb.BrandId == b.Id && cb.CategoryId == categoryId))
                    .Select(b => new LookupItemDto(b.Id, b.Name))
                    .ToListAsync();
            }, slidingExpiration: TimeSpan.FromMinutes(60), tag: "brands");

            return Ok(realBrands);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetModelsByBrand(int categoryId, int brandId)
        {
            var models = await _cacheService.GetOrCreateAsync($"ajax_models_{categoryId}_{brandId}", async () =>
            {
                return await _context.DeviceModels
                    .AsNoTracking()
                    .Where(m => m.CategoryId == categoryId && m.BrandId == brandId)
                    .Select(m => new LookupItemDto(m.Id, m.Name))
                    .ToListAsync();
            }, slidingExpiration: TimeSpan.FromMinutes(60), tag: "models");

            return Ok(models);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPartTypesByCategory(int categoryId)
        {
            var partTypes = await _cacheService.GetOrCreateAsync($"ajax_parttypes_{categoryId}", async () =>
            {
                return await _context.SparePartTypes
                    .AsNoTracking()
                    .Where(pt => _context.CategoryPartTypes.Any(cpt => cpt.PartTypeId == pt.Id && cpt.CategoryId == categoryId))
                    .Select(pt => new LookupItemDto(pt.Id, pt.Name))
                    .ToListAsync();
            }, slidingExpiration: TimeSpan.FromMinutes(60), tag: "parttypes");

            return Ok(partTypes);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetQualityTypes()
        {
            var qualities = await _cacheService.GetOrCreateAsync("ajax_qualitytypes", async () =>
            {
                return await _context.QualityTypes
                    .AsNoTracking()
                    .OrderBy(q => q.Name)
                    .Select(q => new LookupItemDto(q.Id, q.Name))
                    .ToListAsync();
            }, slidingExpiration: TimeSpan.FromMinutes(60), tag: "qualities");

            return Ok(qualities);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _cacheService.GetOrCreateAsync("ajax_categories", async () =>
            {
                return await _context.Categories
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new LookupItemDto(c.Id, c.Name))
                    .ToListAsync();
            }, slidingExpiration: TimeSpan.FromMinutes(60), tag: "categories");

            return Ok(categories);
        }

        /// <summary>
        /// Autocomplete / navbar axtarışı.
        /// QueryBuilder-dən ApplyTextSearch() istifadə edir —
        /// eyni SQL məntiqi, ayrı bir kod yoxdur.
        /// </summary>
        [HttpGet("[action]")]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Ok(new object[] { });

            // ── QueryBuilder ilə dynamic text search ───────────────────────────
            var rawResults = await _context.SpareParts
                .AsNoTracking()
                .Include(p => p.DeviceModel).ThenInclude(m => m.Brand)
                .Include(p => p.PartType)
                .Include(p => p.Images)
                .ApplyStatusFilter(new SparePartSearchQuery
                {
                    Status         = ApprovalStatus.Approved,
                    IncludeDeleted = false,
                    InStockOnly    = false   // autocomplete-də stok şərti yoxdur
                })
                .ApplyTextSearch(q)          // söz-söz AND filtrləmə
                .Take(8)
                .Select(p => new
                {
                    id        = p.Id,
                    title     = p.Title,
                    brand     = p.DeviceModel != null && p.DeviceModel.Brand != null
                                    ? p.DeviceModel.Brand.Name : "",
                    model     = p.DeviceModel != null ? p.DeviceModel.Name : "",
                    price     = p.Price,
                    imagePath = p.Images.FirstOrDefault(i => i.IsMain) != null
                                    ? p.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl
                                    : (p.Images.Any() ? p.Images.First().ImageUrl : "/images/no-image.png")
                })
                .ToListAsync();
            // ───────────────────────────────────────────────────────────────────

            // Bulk URL resolution
            var paths  = rawResults.Select(r => r.imagePath).Where(p => !string.IsNullOrEmpty(p)).Distinct();
            var urlMap = _blobService.GetBulkUrls(paths);

            var results = rawResults.Select(r => new
            {
                r.id, r.title, r.brand, r.model, r.price,
                image = urlMap.TryGetValue(r.imagePath, out var url) ? url : r.imagePath
            });

            return Ok(results);
        }
    }
}

