namespace InnoShop.ProductManagement.Application.Common.Interfaces;

public record CurrentUser(
    Guid Id,
    string Email,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> Roles);