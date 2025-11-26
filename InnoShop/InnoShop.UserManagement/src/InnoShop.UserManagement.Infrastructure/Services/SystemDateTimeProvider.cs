using InnoShop.UserManagement.Domain.Common.Interfaces;

namespace InnoShop.UserManagement.Infrastructure.Services;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
