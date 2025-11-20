using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public record Rating
{
    public static readonly Error CannotHaveRatingDifferentFromOneToFive = Error.Validation(
        "Rating.CannotHaveRatingDifferentFromOneToFive",
        "Rating can be from one to five"
    );
    
    private Rating(int value) => Value = value;

    public int Value { get; init; }
    public static ErrorOr<Rating> Create(int value)
    {
        if (value < 1 || value > 5)
        {
            return CannotHaveRatingDifferentFromOneToFive;
        }
        return new Rating(value);
    }
}
