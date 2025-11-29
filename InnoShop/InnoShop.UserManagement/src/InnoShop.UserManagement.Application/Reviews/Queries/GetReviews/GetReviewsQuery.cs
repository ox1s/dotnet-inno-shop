using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.GetReviews;

[Authorize(Permissions = AppPermissions.Review.Read)]
public record GetReviewsQuery(
    Guid TargetUserId,
    int Page = 1,
    int PageSize = 10
) : IRequest<ErrorOr<List<Review>>>;
