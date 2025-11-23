using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record Rating
{
    public static readonly Error InvalidRange = Error.Validation(
        "Rating.InvalidRange",
        "Rating can be from one to five"
    );

    private Rating(int value) => Value = value;

    public int Value { get; init; }
    public static ErrorOr<Rating> Create(int value)
    {
        return (value < 1 || value > 5) ? InvalidRange : new Rating(value);
    }
}
