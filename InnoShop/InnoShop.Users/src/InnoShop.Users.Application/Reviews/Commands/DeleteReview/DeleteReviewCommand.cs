using ErrorOr;
using MediatR;

namespace InnoShop.Users.Application.Reviews.Commands.DeleteReview;

public record DeleteReviewCommand(
    Guid Id
) : IRequest<ErrorOr<Deleted>>;
