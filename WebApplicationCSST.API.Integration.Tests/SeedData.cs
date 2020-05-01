using System;
using System.Collections.Generic;
using WebApplicationCSST.Data;
using WebApplicationCSST.Repo;

namespace WebApplicationCSST.API.Integration.Tests
{
    public static class SeedData
    {
        public static void PopulateTestData(ApplicationDbContext dbContext)
        {
            foreach (var product in SeedData.GetAllProductInit())
            {
                dbContext.Products.Add(product);
            }

            dbContext.SaveChanges();
        }

        public static List<Product> GetAllProductInit() 
        {
            var products = new List<Product>();

            products.Add(new Product { ProductName = "iPhone XR", ProductDetails = null, AddedDate = new DateTime(2020, 3, 29), ModifiedDate = new DateTime(2020, 3, 29) });
            products.Add(new Product { ProductName = "One Plus 7 Pro", ProductDetails = null, AddedDate = new DateTime(2020, 3, 28), ModifiedDate = new DateTime(2020, 3, 28) });

            return products;
        }
    }
}
