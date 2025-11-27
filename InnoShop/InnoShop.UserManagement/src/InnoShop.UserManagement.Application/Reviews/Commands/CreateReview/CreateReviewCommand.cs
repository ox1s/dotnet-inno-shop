using ErrorOr;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    Guid TargetUserId,
    int Rating,
    string? Comment) : IRequest<ErrorOr<Review>>;