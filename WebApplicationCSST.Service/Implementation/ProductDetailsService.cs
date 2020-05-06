using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApplicationCSST.Data;
using WebApplicationCSST.Repo;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.Service
{
    public class ProductDetailsService : IProductDetailsService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public ProductDetailsService(ILogger<ProductService> logger,
            IMapper mapper,
            IUnitOfWork uow)
        {
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<PagedList<ProductDetailsModel>> GetDetailsProducts(PagingParameters pagingParameters)
        {
            _logger.LogInformation("Srv - GetDetailsProducts");

            var currentListEntities = await PagedList<ProductDetailsModel>.ToPagedList(
                _mapper,
                _uow.GetRepository<ProductDetails>().GetQueryable(),
                pagingParameters.PageNumber,
                pagingParameters.PageSize
                );

            return currentListEntities;
        }
    }
}
