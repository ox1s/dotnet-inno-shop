using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.ReviewAggregate;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.UserManagement.Infrastructure.Persistence.Repositories;

public class ReviewsRepository : IReviewsRepository
{
    private readonly UserManagementDbContext _dbContext;

    public ReviewsRepository(UserManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddReviewAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reviews.AddAsync(review, cancellationToken);
    }

    public async Task<Review?> GetByIdAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);
    }

    public Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        _dbContext.Reviews.Update(review);

        return Task.CompletedTask;
    }
}