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
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(message);

            using var scope = serviceScopeFactory.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            await publisher.Publish(integrationEvent);

            if (_channel != null) await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error handling integration event");
        }
    }
}