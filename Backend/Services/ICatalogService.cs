using EHisseler.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHisseler.Web.Services
{
    public interface ICatalogService
    {
        Task<object> GetCategoriesAsync();
        Task<object> GetProductsAsync(string search, int? categoryId, int? partTypeId);
    }
}
