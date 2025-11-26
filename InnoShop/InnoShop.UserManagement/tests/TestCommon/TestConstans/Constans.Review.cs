using InnoShop.UserManagement.Domain.ReviewAggregate;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.TestCommon.TestConstants;

public static partial class Constants
{
    public static class Review
    {
        public static readonly Guid TargetUserId = Guid.NewGuid();
        public static readonly Guid AuthorId = Guid.NewGuid();
        public const int RatingInt = 1;
        public const string CommentStr = "Ок.";
        public static readonly Rating ValidRating = new Rating(1);
        public static readonly Comment Comment = new Comment("Отлично🚽");
    }

}
