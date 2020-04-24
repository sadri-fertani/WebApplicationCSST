using System;

namespace WebApplicationCSST.Data
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ProductDetails ProductDetails { get; set; }
    }
}
