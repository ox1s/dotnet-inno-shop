using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Contracts.Reviews;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;

[Authorize(Permissions = AppPermissions.Review.Update, Policies = AppPolicies.SelfOrAdmin)]
public record UpdateReviewCommand(
    Guid Id,
    Guid UserId,
    int Rating,
    string? Comment) : IAuthorizeableRequest<ErrorOr<ReviewResponse>>;