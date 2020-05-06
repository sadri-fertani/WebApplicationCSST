using System.Threading.Tasks;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.Service
{
    public interface IProductDetailsService
    {
        Task<PagedList<ProductDetailsModel>> GetDetailsProducts(PagingParameters pagingParameters);
    }
}