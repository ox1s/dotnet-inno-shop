using System.Text.Json.Serialization;
using MediatR;

using InnoShop.SharedKernel.IntegrationEvents.UserManagement;

namespace InnoShop.SharedKernel.IntegrationEvents;

[JsonDerivedType(typeof(UserProfileActivatedIntegrationEvent), typeDiscriminator: nameof(UserProfileActivatedIntegrationEvent))]
[JsonDerivedType(typeof(UserProfileDeactivatedIntegrationEvent), typeDiscriminator: nameof(UserProfileDeactivatedIntegrationEvent))]
[JsonDerivedType(typeof(UserRegisteredIntegrationEvent), typeDiscriminator: nameof(UserRegisteredIntegrationEvent))]
[JsonDerivedType(typeof(UserProfileUpdatedIntegrationEvent), typeDiscriminator: nameof(UserProfileUpdatedIntegrationEvent))]
public interface IIntegrationEvent : INotification { }