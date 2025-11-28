using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.GetReview;

[Authorize(Permissions = AppPermissions.Review.Read)]
public record GetReviewQuery(Guid ReviewId) : IRequest<ErrorOr<Review>>;
