namespace InnoShop.SharedKernel.Security.Permissions;

public static class AppPermissions
{
    public static class User
    {
        public const string Create = "user:create";
        public const string Delete = "user:delete";
        public const string Read = "user:read";
    }

    public static class Review
    {
        public const string Create = "review:create";
        public const string Read = "review:read";
        public const string Update = "review:update";
        public const string Delete = "review:delete";
    }

    public static class UserProfile
    {
        public const string Create = "profile:create";
        public const string Read = "profile:read";
        public const string Update = "profile:update";
        public const string Activate = "profile:activate";
        public const string Deactivate = "profile:deactivate";
    }
}