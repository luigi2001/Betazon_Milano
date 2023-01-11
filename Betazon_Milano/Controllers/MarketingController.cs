using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Betazon_Milano.CenterHubs;
using Betazon_Milano.Hubs;
using Betazon_Milano.Models;

namespace Betazon_Milano.Controllers
{
    // Controller Test Hub SignalR

    [Route("api/[controller]")]
    [ApiController]
    public class MarketingController : Controller
    {

        private IHubContext<CenterHub, IMessageHubClient> messageHub;

        public MarketingController(IHubContext<CenterHub, IMessageHubClient> _messageHub)
        {
            messageHub = _messageHub;
        }

        [HttpPost]
        [Route("marketings")]
        public string Get(Messaggi messaggi)
        {
            List<string> marketings = new List<string>();

            marketings.Add(messaggi.Messaggio);

            messageHub.Clients.All.SendMarketingToUser(marketings);

            return "Aggiornamento Marketing, eseguito con successo";
        }
    }
}
