using System.Text;
using System.Text.Json;
using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.IntegrationEventsPublisher;

public class IntegrationEventsPublisher : IIntegrationEventsPublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<IntegrationEventsPublisher> _logger;
    private readonly int _maxRetries = 3;
    private readonly MessageBrokerSettings _messageBrokerSettings;

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
        ArgumentNullException.ThrowIfNull(integrationEvent);

        var retryCount = 0;
        while (retryCount < _maxRetries)
            try
            {
                using var channel = await _connection.CreateChannelAsync();
                await channel.ExchangeDeclareAsync(
                    _messageBrokerSettings.ExchangeName,
                    ExchangeType.Fanout,
                    true);

                var serializedIntegrationEvent = JsonSerializer.Serialize(integrationEvent);
                var body = Encoding.UTF8.GetBytes(serializedIntegrationEvent);

                var properties = new BasicProperties
                {
                    Persistent = true
                };

                _logger.LogInformation("Publishing integration event: {EventType}", integrationEvent.GetType().Name);

                await channel.BasicPublishAsync(
                    _messageBrokerSettings.ExchangeName,
                    string.Empty,
                    false,
                    properties,
                    body);

                _logger.LogInformation("Integration event published successfully: {EventType}",
                    integrationEvent.GetType().Name);
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
                _logger.LogError(ex, "Failed to publish integration event: {EventType}",
                    integrationEvent.GetType().Name);
                throw;
            }

        throw new Exception($"Failed to publish after {_maxRetries} retries");
    }
}