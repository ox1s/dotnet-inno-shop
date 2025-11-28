using InnoShop.UserManagement.Api.Controllers.Common;
using InnoShop.UserManagement.Application.Users.Commands.CreateUserProfile;
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
            userResult => CreatedAtAction(
                actionName: nameof(GetUserProfile),
                routeValues: new { UserId = userId },
                value: MapToProfileResponse(userResult)),
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
            _ => NoContent(),
            Problem);
    }

    [HttpGet("{userId:guid}/profile")]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        var query = new GetUserProfileQuery(userId);

        var result = await sender.Send(query);

        return result.Match(
            userResult => Ok(MapToProfileResponse(userResult)),
            Problem);
    }
    
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var query = new GetUserQuery(userId);
        var result = await sender.Send(query);

        return result.Match(
            user => Ok(new UserResponse(
                user.Id,
                user.Email.Value,
                user.Roles.Select(r => r.Name).ToList(),
                user.IsEmailVerified,
                user.IsActive,
                user.UserProfile != null ? MapToProfileResponse(user) : null
            )),
            Problem);
    }

    private static UserProfileResponse MapToProfileResponse(User user)
    {
        var profile = user.UserProfile!; 
        
        return new UserProfileResponse(
            UserId: user.Id,
            FirstName: profile.FirstName.Value,
            LastName: profile.LastName.Value,
            AvatarUrl: profile.AvatarUrl.Value,
            PhoneNumber: profile.PhoneNumber.Value,
            Country: profile.Location.Country.Name,
            City: profile.Location.City);
    }
}