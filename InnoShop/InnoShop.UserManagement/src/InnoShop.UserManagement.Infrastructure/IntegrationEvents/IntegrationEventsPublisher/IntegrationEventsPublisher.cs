using System.Text;
using System.Text.Json;
using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;

public class IntegrationEventsPublisher(
    IConnection connection,
    IOptions<MessageBrokerSettings> messageBrokerOptions,
    ILogger<IntegrationEventsPublisher> logger)
    : IIntegrationEventsPublisher
{
    private readonly MessageBrokerSettings _messageBrokerSettings = messageBrokerOptions.Value;

    public async Task PublishEventAsync(IIntegrationEvent integrationEvent)
    {
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            _messageBrokerSettings.ExchangeName,
            ExchangeType.Fanout, true);

        var serializedIntegrationEvent = JsonSerializer.Serialize(integrationEvent);
        var body = Encoding.UTF8.GetBytes(serializedIntegrationEvent);

        logger.LogInformation("Publishing integration event: {EventType}", integrationEvent.GetType().Name);

        await channel.BasicPublishAsync(
            _messageBrokerSettings.ExchangeName,
            string.Empty,
            body);

        logger.LogInformation("Integration event published successfully: {EventType}",
            integrationEvent.GetType().Name);
    }
}