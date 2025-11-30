using InnoShop.UserManagement.Api.Controllers.Common;
using InnoShop.UserManagement.Application.Users.Commands.ActivateUserProfile;
using InnoShop.UserManagement.Application.Users.Commands.CreateUserProfile;
using InnoShop.UserManagement.Application.Users.Commands.DeactivateUserProfile;
using InnoShop.UserManagement.Application.Users.Commands.UpdateUserProfile;
using InnoShop.UserManagement.Application.Users.Queries.GetUser;
using InnoShop.UserManagement.Application.Users.Queries.GetUserProfile;
using InnoShop.UserManagement.Contracts.Users;
using InnoShop.UserManagement.Domain.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.UserManagement.Api.Controllers;

[Route("api/v1/users")]
public class UsersController(ISender sender) : ApiController
{
    [HttpPost("{userId:guid}/profile")]
    public async Task<IActionResult> CreateUserProfile(
        Guid userId,
        [FromBody] CreateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var countryResult = CountryUtils.ToDomain(request.Country);
        if (countryResult.IsError) return Problem(countryResult.Errors);

        var command = new CreateUserProfileCommand(
            UserId: userId,
            FirstName: request.FirstName,
            LastName: request.LastName,
            AvatarUrl: request.AvatarUrl,
            PhoneNumber: request.PhoneNumber,
            Country: countryResult.Value,
            State: request.State,
            City: request.City
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            userProfile => CreatedAtAction(
                actionName: nameof(GetUserProfile),
                routeValues: new { UserId = userId },
                value: userProfile),
            Problem);
    }

    [HttpPut("{userId:guid}/profile")]
    public async Task<IActionResult> UpdateUserProfile(
        Guid userId,
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        Country? country = null;
        if (!string.IsNullOrEmpty(request.Country))
        {
            var countryResult = CountryUtils.ToDomain(request.Country);
            if (countryResult.IsError) return Problem(countryResult.Errors);
            country = countryResult.Value;
        }

        var command = new UpdateUserProfileCommand(
            UserId: userId,
            FirstName: request.FirstName,
            LastName: request.LastName,
            AvatarUrl: request.AvatarUrl,
            PhoneNumber: request.PhoneNumber,
            Country: country,
            State: request.State,
            City: request.City
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            userProfile => Ok(userProfile),
            Problem);
    }

    [HttpGet("{userId:guid}/profile")]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        var query = new GetUserProfileQuery(userId);

        var result = await sender.Send(query);

        return result.Match(
            userProfile => Ok(userProfile),
            Problem);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var query = new GetUserQuery(userId);
        var result = await sender.Send(query);

        return result.Match(
            user => Ok(user),
            Problem);
    }

    [HttpPost("{userId:guid}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeactivateUserProfileCommand(userId);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{userId:guid}/activate")]
    public async Task<IActionResult> ActivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var command = new ActivateUserProfileCommand(userId);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }
}