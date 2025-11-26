using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;

namespace InnoShop.UserManagement.Infrastructure.Persistence.Repositories;

public class ReviewsRepository : IReviewsRepository
{
    public Task AddReviewAsync(Review review, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}
