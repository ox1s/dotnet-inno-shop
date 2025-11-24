using ErrorOr;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    Guid TargetUserId,
    Guid AuthorId,
    int Rating,
    string? Comment
) : IRequest<ErrorOr<Review>>;