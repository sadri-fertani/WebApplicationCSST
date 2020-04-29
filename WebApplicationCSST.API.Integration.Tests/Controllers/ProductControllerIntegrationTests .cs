using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplicationCSST.Service.Models;
using Xunit;

namespace WebApplicationCSST.API.Integration.Tests
{
    public class ProductControllerIntegrationTests : IClassFixture<AppTestFixture>
    {
        readonly HttpClient _client;

        public ProductControllerIntegrationTests(AppTestFixture fixture)
        {
            _client = fixture.CreateClient();
        }

        [Theory]
        [InlineData("/api/Product/1")]
        public async Task GetProduct_Valid_OK(string url)
        {
            // Act
            var response = await _client.GetAsync(url);
            // Assert 1
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Assert 2
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

            // Deserialize and examine results.
            var stringResponse = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductModel>(stringResponse);

            // Assert 3
            Assert.Equal(1, product.Id);

            // Assert 4
            Assert.Equal("iPhone XR", product.ProductName);
        }

        [Theory]
        [InlineData("/api/Product/0")]
        public async Task GetProduct_InValid_NotFound(string url)
        {
            // Act
            var response = await _client.GetAsync(url);
            // Assert 1
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);            
        }
    }
}
