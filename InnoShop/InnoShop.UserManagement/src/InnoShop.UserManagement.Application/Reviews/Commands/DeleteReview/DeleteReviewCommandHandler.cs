using ErrorOr;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler(
    IUnitOfWork _unitOfWork,
    IReviewsRepository _reviewsRepository,
    IDateTimeProvider _dateTimeProvider) : IRequestHandler<DeleteReviewCommand, ErrorOr<Deleted>>
{

    public async Task<ErrorOr<Deleted>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewsRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review is null)
        {
            return ReviewErrors.NotFound;
        }

        var deleteReviewResult = review.Delete(_dateTimeProvider);
        if (deleteReviewResult.IsError)
        {
            return deleteReviewResult.Errors;
        }
        
        await _reviewsRepository.UpdateAsync(review, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Deleted;
    }

}
