using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MSS.API.Core.EventServer
{
    public class MssEventHub : Hub
    {
        public async Task SendWarnning(string message)
        {
            await Clients.All.SendAsync("warnning", message);
        }
    }
}