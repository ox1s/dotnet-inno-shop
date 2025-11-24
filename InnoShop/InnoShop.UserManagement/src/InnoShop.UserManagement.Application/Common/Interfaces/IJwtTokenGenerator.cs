using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}