using Moq;
using NUnit.Framework;
using WebApplicationCSST.API.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using WebApplicationCSST.Service;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebApplicationCSST.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationCSST.Test
{
    public class UnitTestProductController
    {
        private Mock<LinkGenerator> _mockLinkGenerator;
        private Mock<ILogger<ProductController>> _mockLogger;
        private Mock<IProductService> _mockProductService;

        [OneTimeSetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ProductController>>();
            _mockProductService = new Mock<IProductService>();

            // LinkGenerator
            _mockLinkGenerator = new Mock<LinkGenerator>();

            _mockLinkGenerator
                .Setup(
                    g => g.GetPathByAddress(
                        It.IsAny<HttpContext>(),
                        It.IsAny<RouteValuesAddress>(),
                        It.IsAny<RouteValueDictionary>(),
                        It.IsAny<RouteValueDictionary>(),
                        It.IsAny<PathString?>(),
                        It.IsAny<FragmentString>(),
                        It.IsAny<LinkOptions>()
                        )
                )
                .Returns("/");
        }

        [Test]
        public async Task GetOne_WhenCalledWithId_ReturnProductModelResult_Async()
        {
            // Arrange
            ProductModel iPhoneXr = new ProductModel
            {
                Id = 219,
                ProductName = "iPhone XR",
                ProductDetails = null            
            };

            _mockProductService
                .Setup(r => r.GetProduct(219)).Returns(Task.FromResult(iPhoneXr));

            var controller = new ProductController(
                _mockLinkGenerator.Object,
                _mockLogger.Object,
                _mockProductService.Object);

            // Act
            var result = await controller.GetOne(219);

            // Assert
            var actionResult = result;
            Assert.IsNotNull(actionResult);
            Assert.IsAssignableFrom(typeof(ActionResult<ProductModel>), actionResult);

            Assert.That(actionResult.Value, Is.EqualTo(iPhoneXr));
        }
    }
}