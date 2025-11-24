using ErrorOr;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Queries.GetReview;

public record GetReviewQuery(Guid ReviewId) : IRequest<ErrorOr<Review>>;
