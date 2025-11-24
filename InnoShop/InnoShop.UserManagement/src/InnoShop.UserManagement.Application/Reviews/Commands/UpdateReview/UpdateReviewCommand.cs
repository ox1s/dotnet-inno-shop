using ErrorOr;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;

public record UpdateReviewCommand(
    Guid Id,
    int Rating,
    string? Comment
) : IRequest<ErrorOr<Success>>;