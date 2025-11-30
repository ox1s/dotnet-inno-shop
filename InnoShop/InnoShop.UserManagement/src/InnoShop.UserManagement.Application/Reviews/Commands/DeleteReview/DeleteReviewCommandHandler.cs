using ErrorOr;
using InnoShop.SharedKernel.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;

namespace InnoShop.UserManagement.Application.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler(
    IUnitOfWork unitOfWork,
    IReviewsRepository reviewsRepository,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<DeleteReviewCommand, ErrorOr<Deleted>>
{

    public async Task<ErrorOr<Deleted>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await reviewsRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null) return ReviewErrors.NotFound;

        if (review.AuthorId != request.UserId) return UserErrors.NotTheReviewAuthor;

        var deleteReviewResult = review.Delete(dateTimeProvider);
        if (deleteReviewResult.IsError) return deleteReviewResult.Errors;

        await reviewsRepository.UpdateAsync(review, cancellationToken);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Deleted;
    }

}
