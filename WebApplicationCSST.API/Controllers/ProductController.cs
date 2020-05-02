using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationCSST.API.Extensions;
using WebApplicationCSST.API.Provider.Role;
using WebApplicationCSST.Service;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.API.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public const string MC_PRODUCTS = "MC_PRODUCTS";
        public const string DC_PRODUCT = "DC_PRODUCT";

        public ProductController(
            ILogger<ProductController> logger,
            IProductService productService,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _productService = productService;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        [AllowAnonymous]
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ProductModel>> GetOne(long id)
        {
            try
            {
                var product = await _productService.GetProduct(id);
                if (product == null) return NotFound($"Produit ayant l'id: {id} est introuvable.");

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error product-get-one {id} : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = WebApplicationRoleProvider.ADMIN)]
        [EnableQuery(PageSize = 10)]
        [HttpGet()]
        public async Task<ActionResult<List<ProductModel>>> GetAll()
        {
            try
            {
                var products = await _productService.GetProducts();
                if (products == null) return NotFound($"Aucun produit trouvé.");

                return products.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error product-get-all : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost()]
        public async Task<ActionResult<ProductModel>> Post(ProductModel model)
        {
            try
            {
                var newModel = await _productService.InsertProduct(model);

                return CreatedAtAction(nameof(GetOne), new { id = newModel.Id }, newModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error product-add : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut()]
        public async Task<IActionResult> Put(ProductModel model)
        {
            try
            {
                var product = await _productService.GetProduct(model.Id);
                if (product == null) return NotFound($"Le produit ayant l'id: {model.Id} est introuvable.");

                await _productService.UpdateProduct(model);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error product-update : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductModel>> Delete(int id)
        {
            try
            {
                var product = await _productService.GetProduct(id);
                if (product == null) return StatusCode(StatusCodes.Status204NoContent);

                await _productService.DeleteProduct(id);

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error product-delete : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #region Remove me & Clean code <-> Just for demo [Versionning & Cache]
        #region Get one Version 1.1
        [AllowAnonymous]
        [HttpGet("{id:long}")]
        [MapToApiVersion("1.1")]
        public ActionResult<string> GetOneVersion_1_1(long id)
        {
            return $"Just for test {id}";
        }
        #endregion

        #region Cache : ResponseCache, MemoryCache & DistributedCache
        [AllowAnonymous]
        [Route("AllResponseCacheActivated")]
        [HttpGet()]
        [ResponseCache(CacheProfileName = "Default30")]
        public async Task<ActionResult<List<ProductModel>>> GetAllWithResponseCacheActivated()
        {
            try
            {
                var products = await _productService.GetProducts();
                if (products == null) return NotFound($"Aucun produit trouvé.");

                return products.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error product-get-all : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("AllMemoryCacheActivated")]
        [HttpGet()]
        public async Task<ActionResult<List<ProductModel>>> GetAllWithMemoryCacheActivated()
        {
            //Look for cache key.
            if (_memoryCache.TryGetValue(MC_PRODUCTS, out IEnumerable<ProductModel> products))
            {
                _logger.LogInformation("List of products loaded from MemoryCache.");

                return products.ToList();
            }
            else
            {
                try
                {
                    products = await _productService.GetProducts();
                    if (products == null) return NotFound($"Aucun produit trouvé.");

                    // Save data in cache.
                    _memoryCache.Set(
                        MC_PRODUCTS,
                        products,
                        new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(30),
                            Priority = CacheItemPriority.Normal
                        });

                    _logger.LogInformation("List of products loaded from Database.");

                    return products.ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Error product-get-all : {ex.Message}");

                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        [AllowAnonymous]
        [Route("OneDistributedCacheActivated/{id:long}")]
        [HttpGet()]
        public async Task<ActionResult<ProductModel>> GetOneWithDistributedCacheActivated(long id)
        {
            ProductModel product = null;

            try
            {
                product = (await _distributedCache.GetAsync($"{DC_PRODUCT}-{id}")).ToObject<ProductModel>();
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Error get-one-product-from-distributed-cache : {ex.Message}");
            }
            
            try
            {
                if (product == null)
                {
                    product = await _productService.GetProduct(id);
                    if (product == null) return NotFound($"Produit ayant l'id: {id} est introuvable.");

                    _logger.LogInformation($"Product {product.ProductName} loaded from Database.");

                    // Save data in cache.
                    try
                    {
                        _distributedCache.Set(
                            $"{DC_PRODUCT}-{id}",
                            product.ToByteArray<ProductModel>(),
                            new DistributedCacheEntryOptions
                            {
                                AbsoluteExpiration = DateTime.Now.AddMinutes(30),
                                SlidingExpiration = TimeSpan.FromMinutes(10.0)
                            }
                        );
                    }
                    catch(Exception ex)
                    {
                        _logger.LogCritical($"Error set-one-product-to-distributed-cache : {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Product {product.ProductName} loaded from DistributedCache.");
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error product-get-all : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion
        #endregion
    }
}