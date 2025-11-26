using System;
using System.Collections.Generic;
using System.Text;

namespace InnoShop.UserManagement.Infrastructure.IntegrationEvents;

public record OutboxIntegrationEvent(string EventName, string EventContent);