using System.Text;
using System.Text.Json;
using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.UserManagement.Infrastructure.IntegrationEvents.Settings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents.BackgroundServices;

public class ConsumeIntegrationEventsBackgroundService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly ILogger<ConsumeIntegrationEventsBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly MessageBrokerSettings _settings;

    private IChannel? _channel;

    public ConsumeIntegrationEventsBackgroundService(
        ILogger<ConsumeIntegrationEventsBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<MessageBrokerSettings> settings,
        IConnection connection)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _settings = settings.Value;
        _connection = connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(
            _settings.ExchangeName,
            ExchangeType.Fanout,
            true,
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            _settings.QueueName,
            true,
            false,
            false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            _settings.QueueName,
            _settings.ExchangeName,
            string.Empty,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += HandleMessageAsync;

        await _channel.BasicConsumeAsync(
            _settings.QueueName,
            false,
            consumer,
            stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(message);

            if (integrationEvent is null)
            {
                _logger.LogWarning("Received null integration event");
                if (_channel is not null) await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                return;
            }

            _logger.LogInformation("Handling integration event: {EventType}", integrationEvent.GetType().Name);

            using var scope = _serviceScopeFactory.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            await publisher.Publish(integrationEvent);

            if (_channel is not null) await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling integration event");
            if (_channel is not null) await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}