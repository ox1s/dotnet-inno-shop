using InnoShop.UserManagement.Domain.ReviewAggregate;

namespace InnoShop.UserManagement.TestCommon.TestConstants;

public static partial class Constants
{
    public static class Review
    {
        public static readonly Guid TargetUserId = Guid.NewGuid();
        public static readonly Guid AuthorId = Guid.NewGuid();
        public static readonly Rating Rating = new(1);
        public static readonly Comment Comment = new("Отлично🚽");
    }
}