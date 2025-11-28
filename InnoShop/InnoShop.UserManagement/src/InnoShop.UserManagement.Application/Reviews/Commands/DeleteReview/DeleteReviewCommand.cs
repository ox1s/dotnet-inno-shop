using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.SharedKernel.Security.Roles;
using InnoShop.UserManagement.Application.Common.Security;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.DeleteReview;

[Authorize(Permissions = AppPermissions.Review.Delete, Policies = AppPolicies.SelfOrAdmin)]
public record DeleteReviewCommand(
    Guid UserId
) : IAuthorizeableRequest<ErrorOr<Deleted>>;
