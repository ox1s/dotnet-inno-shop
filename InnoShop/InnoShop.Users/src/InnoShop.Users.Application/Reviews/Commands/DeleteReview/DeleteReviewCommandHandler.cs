using ErrorOr;
using InnoShop.Users.Application.Common.Interfaces;
using InnoShop.Users.Domain.Common.Interfaces;
using InnoShop.Users.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.Users.Application.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider) : IRequestHandler<DeleteReviewCommand, ErrorOr<Deleted>>
{

    public async Task<ErrorOr<Deleted>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _usersRepository.GetReviewByIdAsync(request.Id, cancellationToken);
        if (review is null)
        {
            return ReviewErrors.NotFound;
        }

        review.Delete(_dateTimeProvider);

        await _usersRepository.UpdateReviewAsync(review, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Deleted;
    }

}
