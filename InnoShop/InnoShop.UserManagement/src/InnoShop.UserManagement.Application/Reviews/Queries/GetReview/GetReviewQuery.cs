using ErrorOr;
using InnoShop.SharedKernel.Security.Permissions;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Contracts.Reviews;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.GetReview;

[Authorize(Permissions = AppPermissions.Review.Read)]
public record GetReviewQuery(Guid ReviewId) : IRequest<ErrorOr<ReviewResponse>>;
