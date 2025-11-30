using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.UserManagement.Application.Common.Security;

namespace InnoShop.UserManagement.Application.Reviews.Commands.DeleteReview;

[Authorize(Permissions = AppPermissions.Review.Delete, Policies = AppPolicies.SelfOrAdmin)]
public record DeleteReviewCommand(
    Guid UserId,
    Guid ReviewId
) : IAuthorizeableRequest<ErrorOr<Deleted>>;