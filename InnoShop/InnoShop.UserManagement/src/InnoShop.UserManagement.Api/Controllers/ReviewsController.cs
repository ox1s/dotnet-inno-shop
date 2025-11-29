using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;
using InnoShop.UserManagement.Application.Reviews.Commands.DeleteReview;
using InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;
using InnoShop.UserManagement.Application.Reviews.Queries.GetReview;
using InnoShop.UserManagement.Application.Reviews.Queries.GetReviews;
using InnoShop.UserManagement.Contracts.Reviews;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.UserManagement.Api.Controllers;

[Route("api/v1/users/{targetUserId:guid}/reviews")]
public class ReviewsController(ISender sender, ICurrentUserProvider currentUserProvider) : ApiController
{

    [HttpPost]
    public async Task<IActionResult> CreateReview(
        CreateReviewRequest request,
        Guid targetUserId,
        CancellationToken cancellationToken)
    {

        var command = new CreateReviewCommand(
           TargetUserId: targetUserId,
           Rating: request.Rating,
           Comment: request.Comment);

        var createReviewResult = await sender.Send(command, cancellationToken);

        return createReviewResult.Match(
            review => CreatedAtAction(
                nameof(GetReview),
                new { reviewId = review.Id },
                MapToResponse(review)),
                Problem);

    }

    [HttpPut("{reviewId:guid}")]
    public async Task<IActionResult> UpdateReview(Guid reviewId, UpdateReviewRequest request)
    {
        var currentUserId = currentUserProvider.GetCurrentUser().Id;

        var command = new UpdateReviewCommand(
            UserId: currentUserId,
            Id: reviewId,
            Rating: request.Rating,
            Comment: request.Comment);

        var updateReviewResult = await sender.Send(command);

        return updateReviewResult.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        var currentUserId = currentUserProvider.GetCurrentUser().Id;

        var command = new DeleteReviewCommand(
            UserId: currentUserId,
            ReviewId: reviewId);

        var deleteReviewResult = await sender.Send(command);

        return deleteReviewResult.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpGet("{reviewId:guid}")]
    public async Task<IActionResult> GetReview(Guid reviewId)
    {
        var query = new GetReviewQuery(reviewId);

        var getReviewResult = await sender.Send(query);

        return getReviewResult.Match(
            review => Ok(MapToResponse(review)),
            Problem);
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews(
        Guid targetUserId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetReviewsQuery(targetUserId, page, pageSize);

        var getReviewsResult = await sender.Send(query);

        return getReviewsResult.Match(
            reviews => Ok(reviews.Select(MapToResponse)),
            Problem);
    }

    private static ReviewResponse MapToResponse(Review review)
    {
        return new ReviewResponse(
            review.Id,
            review.AuthorId,
            review.TargetUserId,
            review.Rating.Value,
            review.Comment?.Value,
            review.CreatedAt,
            review.UpdatedAt);
    }
}
