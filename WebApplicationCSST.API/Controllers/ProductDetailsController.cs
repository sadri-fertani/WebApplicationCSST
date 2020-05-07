using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationCSST.API.Filters;
using WebApplicationCSST.Service;
using WebApplicationCSST.Service.Models;

namespace WebApplicationCSST.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        private readonly ILogger<ProductDetailsController> _logger;
        private readonly IProductDetailsService _productDetailsService;

        public ProductDetailsController(
            ILogger<ProductDetailsController> logger,
            IProductDetailsService productDetailsService)
        {
            _logger = logger;
            _productDetailsService = productDetailsService;
        }

        [AllowPagingHeader]
        [HttpGet()]
        public async Task<ActionResult<List<ProductDetailsModel>>> GetAll([FromQuery] PagingParameters pagingParameters)
        {
            try
            {
                var detailsProducts = await _productDetailsService.GetDetailsProducts(pagingParameters);
                
                if (detailsProducts == null) return NotFound($"Aucun détail produit trouvé.");

                return detailsProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error productDetails-get-all : {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}