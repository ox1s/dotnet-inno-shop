namespace InnoShop.UserManagement.Contracts.Reviews;

public record CreateReviewRequest(
    int Rating,
    string? Comment);