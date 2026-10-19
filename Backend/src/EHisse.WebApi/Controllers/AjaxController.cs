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

        public AjaxController(AppDbContext context, IBlobStorageService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetBrandsByCategory(int categoryId)
        {
            var realBrands = await _context.DeviceBrands
                .Where(b => _context.CategoryBrands.Any(cb => cb.BrandId == b.Id && cb.CategoryId == categoryId))
                .Select(b => new { id = b.Id, name = b.Name })
                .ToListAsync();

            return Ok(realBrands);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetModelsByBrand(int categoryId, int brandId)
        {
            var models = await _context.DeviceModels
                .Where(m => m.CategoryId == categoryId && m.BrandId == brandId)
                .Select(m => new { id = m.Id, name = m.Name })
                .ToListAsync();

            return Ok(models);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPartTypesByCategory(int categoryId)
        {
            var partTypes = await _context.SparePartTypes
                .Where(pt => _context.CategoryPartTypes.Any(cpt => cpt.PartTypeId == pt.Id && cpt.CategoryId == categoryId))
                .Select(pt => new { id = pt.Id, name = pt.Name })
                .ToListAsync();

            return Ok(partTypes);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetQualityTypes()
        {
            var qualities = await _context.QualityTypes
                .OrderBy(q => q.Name)
                .Select(q => new { id = q.Id, name = q.Name })
                .ToListAsync();

            return Ok(qualities);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new { id = c.Id, name = c.Name })
                .ToListAsync();

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

