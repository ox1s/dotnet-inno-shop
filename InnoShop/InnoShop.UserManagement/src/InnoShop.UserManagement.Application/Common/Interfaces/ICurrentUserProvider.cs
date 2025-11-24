using InnoShop.UserManagement.Application.Common.Models;

namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUser GetCurrentUser();
}