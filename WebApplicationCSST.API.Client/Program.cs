using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplicationCSST.API.Client.Models;

namespace WebApplicationCSST.API.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Any())
            {
                foreach (var arg in args)
                {
                    Console.WriteLine();

                    try
                    {
                        var result = await GetProduct(arg);

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

        public static async Task<ResultModel> GetProduct(string id)
        {
            if(!long.TryParse(id, out long idProduct))
            {
                return new ResultModel
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Result = null
                };
            }

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ClientCertificates.Add(new X509Certificate2("Cert01.cer"));
            handler.SslProtocols = SslProtocols.Tls12;
            // Developement : https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.dangerousacceptanyservercertificatevalidator?view=netcore-3.0
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            HttpClient client = new HttpClient(handler);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Version", "1.0");

            var response = await client.GetAsync($"https://localhost/CSST/api/product/{idProduct}");

            if (!response.IsSuccessStatusCode)
            {
                return new ResultModel
                {
                    StatusCode = response.StatusCode,
                    Result = null
                };
            }
            else
            {
                return new ResultModel
                {
                    StatusCode = response.StatusCode,
                    Result = JsonSerializer.Deserialize<ProductModel>(await response.Content.ReadAsStringAsync())
                };
            }
        }
    }
}
