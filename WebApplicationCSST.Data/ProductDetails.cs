using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationCSST.Data
{
    public class ProductDetails : BaseEntity
    {
        public int StockAvailable { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
