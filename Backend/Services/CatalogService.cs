using EHisseler.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly AppDbContext _context;
        private readonly IBlobStorageService _blobService;

        public CatalogService(AppDbContext context, IBlobStorageService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<object> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new { id = c.Id, name = c.Name, displayOrder = c.DisplayOrder })
                .ToListAsync();
            
            return categories;
        }

        public async Task<object> GetProductsAsync(string search, int? categoryId, int? partTypeId)
        {
            var query = _context.SpareParts
                .Include(p => p.DeviceModel).ThenInclude(m => m.Brand)
                .Include(p => p.PartType)
                .Include(p => p.QualityType)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Where(p => p.ModerationStatus == ApprovalStatus.Approved && p.IsActive && !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.DeviceModel.Name.Contains(search) || p.DeviceModel.Brand.Name.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.DeviceModel.CategoryId == categoryId.Value);
            }

            if (partTypeId.HasValue)
            {
                query = query.Where(p => p.PartTypeId == partTypeId.Value);
            }

            var rawResults = await query.OrderByDescending(p => p.CreatedDate).Take(20).ToListAsync();

            var paths = rawResults.SelectMany(p => p.Images).Select(i => i.ImageUrl).Where(p => !string.IsNullOrEmpty(p)).Distinct();
            var urlMap = _blobService.GetBulkUrls(paths);

            var products = rawResults.Select(p => new
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
                sellerName = p.Seller?.CompanyName ?? p.Seller?.ContactPerson,
                images = p.Images.Select(i => new
                {
                    isMain = i.IsMain,
                    url = urlMap.TryGetValue(i.ImageUrl, out var url) ? url : i.ImageUrl
                }).ToList()
            });

            return products;
        }
    }
}
