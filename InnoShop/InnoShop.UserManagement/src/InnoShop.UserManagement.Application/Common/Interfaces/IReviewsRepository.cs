using InnoShop.UserManagement.Domain.ReviewAggregate;

namespace InnoShop.UserManagement.Application.Common.Interfaces;

public interface IReviewsRepository
{
    Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task AddReviewAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
}