using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Contracts.Reviews;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.ListReviews;

[Authorize(Permissions = AppPermissions.Review.Read)]
public record ListReviewsQuery(
    Guid TargetUserId,
    int Page = 1,
    int PageSize = 10
) : IRequest<ErrorOr<List<ReviewResponse>>>;
