using System;
using WebApplicationCSST.Data;
using WebApplicationCSST.Repo;

namespace WebApplicationCSST.API.Integration.Tests
{
    public static class SeedData
    {
        public static void PopulateTestData(ApplicationDbContext dbContext)
        {
            dbContext.Products.Add(new Product {ProductName = "iPhone XR", ProductDetails = null, AddedDate = new DateTime(2020, 3, 29), ModifiedDate = new DateTime(2020, 3, 29) });
            dbContext.Products.Add(new Product {ProductName = "One Plus 7 Pro", ProductDetails = null, AddedDate = new DateTime(2020, 3, 28), ModifiedDate = new DateTime(2020, 3, 28) });
            dbContext.SaveChanges();
        }
    }
}
