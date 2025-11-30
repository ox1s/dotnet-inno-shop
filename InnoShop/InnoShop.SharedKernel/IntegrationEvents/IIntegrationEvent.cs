using System.Text.Json.Serialization;
using InnoShop.SharedKernel.IntegrationEvents.UserManagement;
using MediatR;

namespace InnoShop.SharedKernel.IntegrationEvents;

[JsonDerivedType(typeof(UserProfileActivatedIntegrationEvent), nameof(UserProfileActivatedIntegrationEvent))]
[JsonDerivedType(typeof(UserProfileDeactivatedIntegrationEvent), nameof(UserProfileDeactivatedIntegrationEvent))]
[JsonDerivedType(typeof(UserRegisteredIntegrationEvent), nameof(UserRegisteredIntegrationEvent))]
[JsonDerivedType(typeof(UserProfileUpdatedIntegrationEvent), nameof(UserProfileUpdatedIntegrationEvent))]
[JsonDerivedType(typeof(PasswordResetRequestedIntegrationEvent), nameof(PasswordResetRequestedIntegrationEvent))]
public interface IIntegrationEvent : INotification
{
}