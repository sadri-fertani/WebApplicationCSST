using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ProductModel>> GetProducts();
        Task<ProductModel> GetProduct(long id);
        Task<ProductModel> InsertProduct(ProductModel model);
        Task UpdateProduct(ProductModel model);
        Task DeleteProduct(long id);
    }
}
