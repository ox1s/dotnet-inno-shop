using InnoShop.UserManagement.Application.Reviews.Commands.CreateReview;
using InnoShop.UserManagement.Application.Reviews.Commands.DeleteReview;
using InnoShop.UserManagement.Application.Reviews.Commands.UpdateReview;
using InnoShop.UserManagement.Application.Reviews.Queries.GetReview;
using InnoShop.UserManagement.Contracts.Reviews;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.UserManagement.Api.Controllers;

[Route("api/v1/users/{targetUserId:guid}/reviews")] 
public class ReviewsController(ISender _sender) : ApiController
{

    [HttpPost]
    public async Task<IActionResult> CreateReview(
        CreateReviewRequest request,
        Guid targetUserId,
        CancellationToken cancellationToken)
    {
        var requestUserId = Guid.Parse(HttpContext.User.Claims.First(claim => claim.Type == "id").Value);

        var command = new CreateReviewCommand(
           TargetUserId: targetUserId,
           AuthorId: requestUserId,
           Rating: request.Rating,
           Comment: request.Comment);

        var createReviewResult = await _sender.Send(command, cancellationToken);

        return createReviewResult.Match(
            review => CreatedAtAction(
                nameof(GetReview),
                new { reviewId = review.Id },
                MapToResponse(review)),
                Problem);

    }

    [HttpPut("reviews/{reviewId:guid}")]
    public async Task<IActionResult> UpdateReview(Guid reviewId, UpdateReviewRequest request)
    {
        var command = new UpdateReviewCommand(
            Id: reviewId,
            Rating: request.Rating,
            Comment: request.Comment);

        var updateReviewResult = await _sender.Send(command);

        return updateReviewResult.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpDelete("reviews/{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        var command = new DeleteReviewCommand(reviewId);

        var deleteReviewResult = await _sender.Send(command);

        return deleteReviewResult.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpGet("reviews/{reviewId:guid}")]
    public async Task<IActionResult> GetReview(Guid reviewId)
    {
        var query = new GetReviewQuery(reviewId);

        var getReviewResult = await _sender.Send(query);

        return getReviewResult.Match(
            review => Ok(MapToResponse(review)),
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
