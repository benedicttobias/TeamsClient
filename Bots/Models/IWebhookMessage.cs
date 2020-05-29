namespace Bots.Models
{
    public interface IWebhookMessage
    {
        string Summary { get; set; }
        string Title { get; set; }
        string MediaType { get; set; }
    }
}