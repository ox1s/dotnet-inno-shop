using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;

public class IntegrationEventsPublisher : IIntegrationEventsPublisher
{
    private readonly MessageBrokerSettings _messageBrokerSettings;
    private readonly IConnection _connection;
    private readonly ILogger<IntegrationEventsPublisher> _logger;
    private readonly int _maxRetries = 3;

    public IntegrationEventsPublisher(
        IConnection connection,
        IOptions<MessageBrokerSettings> messageBrokerOptions,
        ILogger<IntegrationEventsPublisher> logger)
    {
        _connection = connection;
        _messageBrokerSettings = messageBrokerOptions.Value;
        _logger = logger;
    }

    public async Task PublishEventAsync(IIntegrationEvent integrationEvent)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent, nameof(integrationEvent));

        int retryCount = 0;
        while (retryCount < _maxRetries)
        {
            try
            {
                using var channel = await _connection.CreateChannelAsync();
                await channel.ExchangeDeclareAsync(
                    exchange: _messageBrokerSettings.ExchangeName,
                    type: ExchangeType.Fanout,
                    durable: true);

                string serializedIntegrationEvent = JsonSerializer.Serialize(integrationEvent);
                byte[] body = Encoding.UTF8.GetBytes(serializedIntegrationEvent);

                var properties = new BasicProperties
                {
                    Persistent = true
                };

                _logger.LogInformation("Publishing integration event: {EventType}", integrationEvent.GetType().Name);

                await channel.BasicPublishAsync(
                    exchange: _messageBrokerSettings.ExchangeName,
                    routingKey: string.Empty,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Integration event published successfully: {EventType}", integrationEvent.GetType().Name);
                return;
            }
            catch (BrokerUnreachableException ex)
            {
                retryCount++;
                _logger.LogWarning(ex, "RabbitMQ unreachable, retry {Retry}/{Max}", retryCount, _maxRetries);
                await Task.Delay(1000 * retryCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish integration event: {EventType}", integrationEvent.GetType().Name);
                throw;
            }
        }

        throw new Exception($"Failed to publish after {_maxRetries} retries");
    }
}