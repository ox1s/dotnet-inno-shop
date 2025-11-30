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

    public async Task<List<Review>> GetByTargetUserIdAsync(Guid targetUserId, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews
            .Where(r => r.TargetUserId == targetUserId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetByAuthorAndTargetAsync(Guid authorId, Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reviews
            .FirstOrDefaultAsync(r => r.AuthorId == authorId && r.TargetUserId == targetUserId, cancellationToken);
    }
}