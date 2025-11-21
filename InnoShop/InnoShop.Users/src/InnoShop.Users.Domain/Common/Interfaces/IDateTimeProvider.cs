using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnoShop.Users.Domain.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}