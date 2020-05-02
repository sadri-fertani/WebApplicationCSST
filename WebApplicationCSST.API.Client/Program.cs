using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApplicationCSST.API.Client.Models;
using WebApplicationCSST.API.Client.Services;

namespace WebApplicationCSST.API.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Any())
            {
                var srv = new ProductService();

                foreach (var arg in args)
                {
                    Console.WriteLine();

                    try
                    {
                        var result = await srv.GetProduct(arg);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            var product = result.Result as ProductModel;

                            Console.WriteLine("---------------------");
                            Console.WriteLine($"Name : {product.ProductName}");
                            Console.WriteLine($"Price : {product.ProductDetails?.Price}");
                            Console.WriteLine($"Stock : {product.ProductDetails?.StockAvailable}");
                            Console.WriteLine("---------------------");
                        }
                        else
                        {
                            Console.WriteLine("---------------------");
                            Console.WriteLine(result.StatusCode.ToString());
                            Console.WriteLine("---------------------");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                Console.WriteLine();
            }
            else
                throw new ArgumentException("No arguments.");
        }
    }
}
