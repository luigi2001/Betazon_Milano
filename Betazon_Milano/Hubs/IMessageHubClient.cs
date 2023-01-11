namespace Betazon_Milano.Hubs
{
    public interface IMessageHubClient
    {
        Task SendMarketingToUser(List<string> offers);
    }
}
