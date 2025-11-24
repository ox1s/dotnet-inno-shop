using ErrorOr;

namespace InnoShop.UserManagement.Domain.ReviewAggregate;

public sealed record Rating(int Value)
{
    public static ErrorOr<Rating> Create(int value)
    {
        return (value < 1 || value > 5) ?
            RatingErrors.InvalidRange
            : new Rating(value);
    }

}

