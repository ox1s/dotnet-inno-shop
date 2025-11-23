using ErrorOr;
using InnoShop.Users.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.Users.Application.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    Guid TargetUserId,
    Guid AuthorId,
    int Rating,
    string? Comment
): IRequest<ErrorOr<Review>>;