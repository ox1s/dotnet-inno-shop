using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Application.Common.Security;
using MediatR;

using InnoShop.UserManagement.Domain.ReviewAggregate;


namespace InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;

[Authorize(Permissions = AppPermissions.Review.Create)]
public record CreateReviewCommand(
    Guid TargetUserId,
    int Rating,
    string? Comment) : IRequest<ErrorOr<Review>>;