using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApplicationCSST.Service.Models;
using Xunit;
using Xunit.Extensions.Ordering;

namespace WebApplicationCSST.API.Integration.Tests
{
    public class ProductControllerIntegrationTests : IClassFixture<AppTestFixture>
    {
        readonly HttpClient _client;

        public ProductControllerIntegrationTests(AppTestFixture fixture)
        {
            _client = fixture.CreateClient();
        }

        [Theory, Order(0)]
        [InlineData("/api/Product")]
        public async Task GetAllProducts_Valid_OK(string url)
        {
            // Act
            var response = await _client.GetAsync(url);
            // Assert 1
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            // Assert 2
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

            // Deserialize and examine results.
            var stringResponse = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<ProductModel>>(stringResponse);

            // Assert 3
            Assert.True(products.Count >= 2);
        }

        [Theory, Order(1)]
        [InlineData("/api/Product/2")]
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
            Assert.Equal(2, product.Id);

            // Assert 4
            Assert.Equal("One Plus 7 Pro", product.ProductName);
        }

        [Theory, Order(2)]
        [InlineData("/api/Product/0")]
        public async Task GetProduct_InValid_NotFound(string url)
        {
            // Act
            var response = await _client.GetAsync(url);
            // Assert 1
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory, Order(3)]
        [InlineData("/api/Product")]
        public async Task PostProduct_Valid_OK(string url)
        {
            // Arrange
            var product = new ProductModel
            {
                ProductName = "Nuraphone",
                ProductDetails = new ProductDetailsModel
                {
                    Price = 10,
                    StockAvailable = 25
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(url, content);
            // Assert 1
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Deserialize and examine results.
            var stringResponse = await response.Content.ReadAsStringAsync();
            var newProduct = JsonConvert.DeserializeObject<ProductModel>(stringResponse);

            // Assert 3
            Assert.NotEqual(0, newProduct.Id);

            // Assert 4
            Assert.Equal("Nuraphone", newProduct.ProductName);
        }

        [Theory, Order(4)]
        [InlineData("/api/Product")]
        public async Task PutProduct_Valid_OK(string url)
        {
            // Arrange
            var product = new ProductModel
            {
                Id = 1,
                ProductName = "iPhone XR",
                ProductDetails = new ProductDetailsModel
                {
                    Id = 0,
                    Price = 10,
                    StockAvailable = 25
                }
            };
            product.ProductName = $"Sadri - {product.ProductName}";

            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync(url, content);
            // Assert 1
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var responseGet = await _client.GetAsync($"{url}/{product.Id}");

            // Deserialize and examine results.
            var stringResponse = await responseGet.Content.ReadAsStringAsync();
            var updatedProduct = JsonConvert.DeserializeObject<ProductModel>(stringResponse);

            // Assert 2
            Assert.Equal(updatedProduct.Id, product.Id);

            // Assert 3
            Assert.Equal(updatedProduct.ProductName, product.ProductName);
        }
                
        [Theory, Order(5)]
        [InlineData("/api/Product")]
        public async Task DeleteProduct_Valid_OK(string url)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(
                    new ProductModel
                    {
                        ProductName = "Parker",
                        ProductDetails = new ProductDetailsModel
                        {
                            Price = 1,
                            StockAvailable = 10
                        }
                    }), 
                Encoding.UTF8, 
                "application/json");

            // Act
            var response = await _client.PostAsync(url, content);
            // Assert 1
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Deserialize and examine results.
            var stringResponse = await response.Content.ReadAsStringAsync();
            var newProduct = JsonConvert.DeserializeObject<ProductModel>(stringResponse);

            // Act
            await _client.DeleteAsync($"{url}/{newProduct.Id}");            
            var responseGet = await _client.GetAsync($"{url}/{newProduct.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, responseGet.StatusCode);
        }
    }
}
