using ErrorOr;

namespace UserManagement.Domain;

public class UserProfile
{
    private readonly Guid _userId;

    internal UserProfile(Guid userId)
    {
        _userId = userId;
    }

}

