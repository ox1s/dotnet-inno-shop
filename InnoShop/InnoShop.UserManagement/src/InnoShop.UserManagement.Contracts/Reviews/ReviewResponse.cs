namespace InnoShop.UserManagement.Contracts.Reviews;

public record ReviewResponse(
    Guid Id,
    Guid AuthorId,
    Guid TargetUserId,
    int Rating,
    string? Comment,
    DateTime CreatedAt,
    DateTime? UpdatedAt);