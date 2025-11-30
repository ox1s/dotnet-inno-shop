namespace InnoShop.ProductManagement.Infrastructure.IntegrationEvents;

public record OutboxIntegrationEvent(string EventName, string EventContent);