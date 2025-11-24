namespace InnoShop.UserManagement.Contracts.Reviews;

public record UpdateReviewRequest(
    int Rating,
    string? Comment);
