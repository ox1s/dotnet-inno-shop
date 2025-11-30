using InnoShop.SharedKernel.IntegrationEvents;
using InnoShop.ProductManagement.Infrastructure.IntegrationEvents.Settings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InnoShop.ProductManagement.Infrastructure.IntegrationEvents.BackgroundServices;

public class ConsumeIntegrationEventsBackgroundService(
    ILogger<ConsumeIntegrationEventsBackgroundService> logger,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<MessageBrokerSettings> settings,
    IConnection connection)
    : BackgroundService
{
    private readonly MessageBrokerSettings _settings = settings.Value;

    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueBindAsync(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: string.Empty,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += HandleMessageAsync;

        await _channel.BasicConsumeAsync(
            queue: _settings.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

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
                logger.LogWarning("Received null integration event");
                if (_channel is not null)
                {
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                return;
            }

            logger.LogInformation("Handling integration event: {EventType}", integrationEvent.GetType().Name);

            using var scope = serviceScopeFactory.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            await publisher.Publish(integrationEvent);

            if (_channel is not null)
            {
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error handling integration event");
            if (_channel is not null)
            {
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}
