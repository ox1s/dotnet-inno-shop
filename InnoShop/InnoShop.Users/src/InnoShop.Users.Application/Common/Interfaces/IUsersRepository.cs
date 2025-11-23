using InnoShop.Users.Domain.ReviewAggregate;
using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.Application.Common.Interfaces;

public interface IUsersRepository
{
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateUserRatingAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Review?> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task AddReviewAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateReviewAsync(Review review, CancellationToken cancellationToken = default);

}
