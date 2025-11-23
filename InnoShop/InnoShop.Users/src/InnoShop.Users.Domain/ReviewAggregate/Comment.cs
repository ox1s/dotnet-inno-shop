using ErrorOr;

namespace InnoShop.Users.Domain.ReviewAggregate;

public sealed record Comment(string Value)
{
    public static ErrorOr<Comment> Create(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return CommentErrors.Empty;
        }

        var trimmed = rawValue.Trim();
        if (trimmed.Length > 1000)
        {
            return CommentErrors.TooLong;
        }
        return new Comment(trimmed);
    }
}