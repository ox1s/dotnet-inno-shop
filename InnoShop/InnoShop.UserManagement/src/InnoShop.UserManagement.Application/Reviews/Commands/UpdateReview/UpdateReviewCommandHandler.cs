using ErrorOr;
using MediatR;

using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;

namespace InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler(
    IReviewsRepository _reviewsRepository,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider) : IRequestHandler<UpdateReviewCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewsRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review is null) return ReviewErrors.NotFound;


        var ratingResult = Rating.Create(request.Rating);
        if (ratingResult.IsError) return ratingResult.Errors;
        var rating = ratingResult.Value;


        Comment? comment = null;
        if (request.Comment is not null)
        {
            var commentResult = Comment.Create(request.Comment);
            if (commentResult.IsError) return commentResult.Errors;
            comment = commentResult.Value;
        }

        var reviewResult = review.Update(
            rating,
            comment ?? null,
            _dateTimeProvider
        );
        if (reviewResult.IsError) return reviewResult.Errors;
        

        await _reviewsRepository.UpdateAsync(review, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success;
    }

}
