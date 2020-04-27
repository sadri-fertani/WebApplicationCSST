using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using WebApplicationCSST.Service;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(
            LinkGenerator linkGenerator,
            ILogger<ProductController> logger,
            IProductService productService)
        {
            _linkGenerator = linkGenerator;
            _logger = logger;
            _productService = productService;
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

        [Authorize]
        [EnableQuery(PageSize = 10)]
        [HttpGet()]
        public async Task<ActionResult<List<ProductModel>>> GetAll()
        {
            try
            {
                var user = this.User.Identity;

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

                var url = _linkGenerator.GetPathByAction(HttpContext,
                      "GetOne",
                      values: new { id = newModel.Id });

                return Created(url, newModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error product-add : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut()]
        public async Task<ActionResult<ProductModel>> Put(ProductModel model)
        {
            try
            {
                var product = await _productService.GetProduct(model.Id);
                if (product == null) return NotFound($"Le produit ayant l'id: {model.Id} est introuvable.");

                await _productService.UpdateProduct(model);

                var url = _linkGenerator.GetPathByAction(HttpContext,
                      "GetOne",
                      values: new { id = product.Id });

                return Created(url, model);
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

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error product-delete : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
