using System.Net;
using System.Threading.Tasks;
using WebApplicationCSST.API.Client.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace WebApplicationCSST.API.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;
        private HttpClientHandler _handler 
        { 
            get 
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ClientCertificates.Add(new X509Certificate2("Cert01.cer"));
                handler.SslProtocols = SslProtocols.Tls12;
                // Developement : https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.dangerousacceptanyservercertificatevalidator?view=netcore-3.0
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                return handler;
            } 
        }

        private const string API_BASE_URL = "https://localhost/CSST/api/product";

        public ProductService()
        {
            _client = new HttpClient(_handler);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("X-Version", "1.0");
        }

        public async Task<ResultModel> GetProduct(string id)
        {
            if (!long.TryParse(id, out long idProduct))
            {
                return new ResultModel
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Result = null
                };
            }

            var response = await _client.GetAsync($"{API_BASE_URL}/{idProduct}");

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
