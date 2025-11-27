using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace InnoShop.UserManagement.Application.Common.Security.Request;
public interface IAuthorizeableRequest<T> : IRequest<T>
{
    Guid UserId { get; }
}