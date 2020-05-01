using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplicationCSST.Data;

namespace WebApplicationCSST.Repo
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _config;

        public ApplicationDbContext(DbContextOptions options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetails> ProductsDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<ProductDetails>().ToTable("ProductDetails");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                  _config.GetConnectionString("DefaultConnection"),
                  b => b.MigrationsAssembly("WebApplicationCSST.API"));
            }
        }
    }
}
