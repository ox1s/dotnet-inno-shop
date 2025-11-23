using ErrorOr;
using InnoShop.Users.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.Users.Application.Reviews.Commands.UpdateReview;

public record UpdateReviewCommand(
    Guid Id,
    int Rating,
    string? Comment
) : IRequest<ErrorOr<Success>>;