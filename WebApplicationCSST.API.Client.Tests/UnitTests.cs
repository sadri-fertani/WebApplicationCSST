using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using WebApplicationCSST.API.Client.Models;

namespace WebApplicationCSST.API.Client.Tests
{
    public class UnitTests
    {
        [Test]
        public async Task Test1_EmptyArgument_ReturnBadRequest()
        {
            // Act
            var result = await Program.GetProduct(string.Empty);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Test2_InvalidArgument_ReturnBadRequest()
        {
            // Act
            var result = await Program.GetProduct("...");

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Test3_ValidArgument_PaysInfo()
        {
            // Arrange
            var idProduct = 1;

            // Act
            var result = await Program.GetProduct(idProduct.ToString());
            var resultProduct = result.Result as ProductModel;

            // Assert
            Assert.That(resultProduct.Id, Is.EqualTo(idProduct));
            Assert.That(resultProduct.ProductName, Is.Not.Empty);
        }

    }
}