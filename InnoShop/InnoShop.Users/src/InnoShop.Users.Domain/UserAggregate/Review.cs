using InnoShop.Users.Domain.Common;

namespace InnoShop.Users.Domain.UserAggregate;

public class Review : Entity
{
    private readonly Guid AuthorId;
    private readonly Rating Rating;
    private readonly string Comment;
    
}
