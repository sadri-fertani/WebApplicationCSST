using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationCSST.Service.Models
{
    public partial class ProductDetailsModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le stock disponible est obligatoire.")]
        [Range(0, 9999, ErrorMessage = "Le stock disponible n'est pas valide.")]
        public int StockAvailable { get; set; }

        [Required(ErrorMessage = "Le prix est obligatoire.")]
        [Range(0.01, 9999.99, ErrorMessage ="Le prix n'est pas valide.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
