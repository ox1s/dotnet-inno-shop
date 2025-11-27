using ErrorOr;
using MediatR;

using InnoShop.UserManagement.Domain.UserAggregate;

namespace InnoShop.UserManagement.Application.Users.Queries.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<ErrorOr<User>>;

