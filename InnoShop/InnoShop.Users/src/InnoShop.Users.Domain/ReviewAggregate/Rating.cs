using ErrorOr;

namespace InnoShop.Users.Domain.ReviewAggregate;

public sealed record Rating(int Value)
{
    public static ErrorOr<Rating> Create(int value)
    {
        return (value < 1 || value > 5) ?
            RatingErrors.InvalidRange
            : new Rating(value);
    }

}

