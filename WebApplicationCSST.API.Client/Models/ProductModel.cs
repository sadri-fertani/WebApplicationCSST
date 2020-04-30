using System.Text.Json.Serialization;

namespace WebApplicationCSST.API.Client.Models
{
    public partial class ProductModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("productDetails")]
        public virtual ProductDetailsModel ProductDetails { get; set; }
    }
}
