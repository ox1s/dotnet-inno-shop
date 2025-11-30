using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Contracts.Reviews;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;

[Authorize(Permissions = AppPermissions.Review.Create)]
public record CreateReviewCommand(
    Guid TargetUserId,
    int Rating,
    string? Comment) : IRequest<ErrorOr<ReviewResponse>>;