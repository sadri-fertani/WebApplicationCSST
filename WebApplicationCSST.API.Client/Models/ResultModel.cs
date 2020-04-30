using System.Net;

namespace WebApplicationCSST.API.Client.Models
{
    public class ResultModel
    {
        public HttpStatusCode StatusCode { get; set; }

        public object Result { get; set; }
    }
}
