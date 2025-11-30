namespace InnoShop.ProductManagement.Infrastructure.IntegrationEvents.Settings;

public class MessageBrokerSettings
{
    public const string Section = "MessageBroker";
    public string QueueName { get; set; } = "product-management-queue";
    public string ExchangeName { get; set; } = "innoshop-events";
}