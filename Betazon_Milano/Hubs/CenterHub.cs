using Microsoft.AspNetCore.SignalR;
using Betazon_Milano.Hubs;

namespace Betazon_Milano.CenterHubs
{
    public class CenterHub : Hub<IMessageHubClient>
    {
    }
}
