using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationCSST.Data;
using WebApplicationCSST.Repo;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.Service
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public ProductService(ILogger<ProductService> logger,
            IMapper mapper,
            IUnitOfWork uow)
        {
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ProductModel> GetProduct(long id)
        {
            var currentProduct = await _uow.GetRepository<Product>().GetAsync(id);
            
            return _mapper.Map<ProductModel>(currentProduct);
        }

        public async Task<IEnumerable<ProductModel>> GetProducts()
        {
            var currentProducts = await _uow.GetRepository<Product>().GetAsync();

            return _mapper.Map<IEnumerable<ProductModel>>(currentProducts);
        }

        public async Task<ProductModel> InsertProduct(ProductModel model)
        {
            var product = _mapper.Map<Product>(model);

            // Init fields (AddedDate & ModifiedDate)
            product.AddedDate = DateTime.Now;
            product.ModifiedDate = product.AddedDate;

            _uow.GetRepository<Product>().Add(product);

            await _uow.CommitAsync();

            return _mapper.Map<ProductModel>(product);
        }

        public async Task UpdateProduct(ProductModel model)
        {
            var oldProduct = await _uow.GetRepository<Product>().GetAsync(model.Id);

            var product = _mapper.Map<Product>(model);

            // Init fields (AddedDate & ModifiedDate)
            product.ModifiedDate = DateTime.Now;
            product.AddedDate = oldProduct.AddedDate;

            _uow.GetRepository<Product>().Update(product);

            await _uow.CommitAsync();
        }

        public async Task DeleteProduct(long id)
        {
            var product = await _uow.GetRepository<Product>().GetAsync(id);
            if (product == null) throw new ArgumentException($"Product with id {id} unfound");

            _uow.GetRepository<Product>().Delete(product);

            await _uow.CommitAsync();
        }
    }
}
