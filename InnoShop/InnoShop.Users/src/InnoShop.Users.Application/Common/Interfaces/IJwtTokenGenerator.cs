using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}