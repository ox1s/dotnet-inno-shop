using ErrorOr;
using InnoShop.SharedKernel.Security.Policies;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Application.Common.Security;
using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Infrastructure.Security.PolicyEnforcer;

public class PolicyEnforcer : IPolicyEnforcer
{
    public ErrorOr<Success> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy)
    {
        return policy switch
        {
            AppPolicies.SelfOrAdmin => SelfOrAdminPolicy(request, currentUser),
            _ => Error.Unexpected(description: "Unknown policy name")
        };
    }

    private static ErrorOr<Success> SelfOrAdminPolicy<T>(IAuthorizeableRequest<T> request, CurrentUser currentUser)
    {
        return request.UserId == currentUser.Id || currentUser.Roles.Contains(Role.Admin.Name)
            ? Result.Success
            : Error.Forbidden(description: "You are not allowed to access this resource.");
    }
}