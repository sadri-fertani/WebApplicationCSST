using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebApplicationCSST.API.Hubs.Models;

namespace WebApplicationCSST.API.Hubs
{
    public class MessageHub : Hub
    {
        public async Task NewMessage(MessageModel msg)
        {
            await Clients.Others.SendAsync("MessageReceived", msg);
        }
    }
}