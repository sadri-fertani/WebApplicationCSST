using System.Threading.Tasks;

namespace WebApplicationCSST.Service
{
    public interface IEmailService
    {
        Task Send(string toAdresse, string toUsername, string messageHTML);
    }
}
