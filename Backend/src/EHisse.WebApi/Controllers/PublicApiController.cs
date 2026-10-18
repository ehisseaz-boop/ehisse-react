using EHisse.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EHisse.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicApiController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public PublicApiController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _catalogService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] int? categoryId, [FromQuery] int? partTypeId)
        {
            var products = await _catalogService.GetProductsAsync(search, categoryId, partTypeId);
            return Ok(products);
        }
    }
}
