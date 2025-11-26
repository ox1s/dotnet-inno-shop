using System.Text.Json.Serialization;
using MediatR;

using InnoShop.SharedKernel.IntegrationEvents.UserManagement;

namespace InnoShop.SharedKernel.IntegrationEvents;

[JsonDerivedType(typeof(UserProfileActivatedIntegrationEvent), typeDiscriminator: nameof(UserProfileActivatedIntegrationEvent))]
[JsonDerivedType(typeof(UserProfileDeactivatedIntegrationEvent), typeDiscriminator: nameof(UserProfileDeactivatedIntegrationEvent))]
public interface IIntegrationEvent : INotification { }