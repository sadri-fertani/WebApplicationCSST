using Moq;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using WebApplicationCSST.API.Client.Models;
using WebApplicationCSST.API.Client.Services;

namespace WebApplicationCSST.API.Client.Unit.Tests
{
    public class Tests
    {
        private Mock<IProductService> _mockProductService;

        [OneTimeSetUp]
        public void Setup()
        {
            _mockProductService = new Mock<IProductService>();
        }

        [Test]
        public async Task Test1_ValidArgument_ProductInfo()
        {
            // Arrange
            var iPhone = new ProductModel 
            {
                Id = 22,
                ProductName = "iPhone XR",
                ProductDetails = new ProductDetailsModel 
                {
                    Id = 22,
                    Price = 145,
                    StockAvailable = 20
                }
            };

            _mockProductService
                .Setup(r => r.GetProduct("22")).Returns(Task.FromResult(new ResultModel { Result = iPhone, StatusCode = HttpStatusCode.OK }));

            // Act
            var result = await _mockProductService.Object.GetProduct("22");

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultProduct = result.Result as ProductModel;

            // Assert
            Assert.That(resultProduct, Is.Not.Null);
            Assert.That(resultProduct, Is.EqualTo(iPhone));
        }
    }
}