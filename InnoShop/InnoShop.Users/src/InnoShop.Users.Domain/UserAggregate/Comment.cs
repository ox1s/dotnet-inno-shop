using ErrorOr;

namespace InnoShop.Users.Domain.UserAggregate;

public sealed record Comment
{
    public string Value { get; }
    private Comment(string value) => Value = value;
    public static ErrorOr<Comment> Create(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return Error.Validation("Comment.Empty", "Comment cannot be empty.");

        var trimmed = rawValue.Trim();
        if (trimmed.Length > 1000)
            return Error.Validation("Comment.TooLong", "Comment must be under 1000 characters.");

        return new Comment(trimmed);
    }
}