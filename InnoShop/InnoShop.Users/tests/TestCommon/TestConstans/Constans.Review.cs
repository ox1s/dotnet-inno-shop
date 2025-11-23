using InnoShop.Users.Domain.ReviewAggregate;
using InnoShop.Users.Domain.UserAggregate;

namespace InnoShop.Users.TestCommon.TestConstants;

public static partial class Constants
{
    public static class Review
    {
        public static readonly Rating ValidRating = new Rating(1);
        public static readonly Comment Comment = new Comment("Отлично🚽");
    }

}
