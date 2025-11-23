using InnoShop.Users.Application.Common.Models;

namespace InnoShop.Users.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUser GetCurrentUser();
}