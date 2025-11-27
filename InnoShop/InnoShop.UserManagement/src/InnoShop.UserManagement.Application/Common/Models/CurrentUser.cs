namespace InnoShop.UserManagement.Application.Common.Interfaces;

public record CurrentUser(
    Guid Id,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> Roles);