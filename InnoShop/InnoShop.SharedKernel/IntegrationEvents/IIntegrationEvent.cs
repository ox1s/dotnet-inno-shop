namespace InnoShop.SharedKernel.IntegrationEvents;

public interface IIntegrationEvent
{
    string EventType { get; }
    int Version { get; }
}
