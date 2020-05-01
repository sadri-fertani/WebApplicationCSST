using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationCSST.Service.Models
{
    [Serializable]
    public partial class ProductModel
    {
        public long Id { get; set; }

        [StringLength(100, ErrorMessage = "Le nom du produit ne doit pas dépasser 100 caratères.")]
        [Required(ErrorMessage ="Le nom du produit est obligatoire.")]
        public string ProductName { get; set; }

        public virtual ProductDetailsModel ProductDetails { get; set; }
    }
}
