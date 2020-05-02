using System.Threading.Tasks;
using WebApplicationCSST.API.Client.Models;

namespace WebApplicationCSST.API.Client.Services
{
    public interface IProductService
    {
        public Task<ResultModel> GetProduct(string id);
    }
}
