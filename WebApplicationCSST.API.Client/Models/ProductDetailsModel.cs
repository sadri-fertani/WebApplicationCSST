using System.Text.Json.Serialization;

namespace WebApplicationCSST.API.Client.Models
{
    public partial class ProductDetailsModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("stockAvailable")]
        public int StockAvailable { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
