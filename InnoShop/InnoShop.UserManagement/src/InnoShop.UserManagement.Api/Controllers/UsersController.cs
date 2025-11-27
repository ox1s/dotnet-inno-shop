
using MediatR;
using Microsoft.AspNetCore.Mvc;

using InnoShop.UserManagement.Api.Controllers;
using InnoShop.UserManagement.Api.Controllers.Common;
using InnoShop.UserManagement.Application.Users.Commands.CreateUserProfile;
using InnoShop.UserManagement.Application.Users.Commands.UpdateUserProfile;
using InnoShop.UserManagement.Contracts.Users;
using InnoShop.UserManagement.Domain.UserAggregate;
using InnoShop.UserManagement.Application.Users.Queries.GetUserProfile;

[Route("api/v1/users")]
public class UsersController(ISender _sender) : ApiController
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

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            userResult => CreatedAtAction(
                actionName: nameof(GetUserProfile),
                routeValues: new { UserId = userId },
                value: MapToResponse(userResult)),
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

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpGet("{userId:guid}/profile")]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        var query = new GetUserProfileQuery(userId);

        var result = await _sender.Send(query);

        return result.Match(
            userResult => Ok(MapToResponse(userResult)),
            Problem);
    }
    private static UserProfileResponse MapToResponse(User user)
    {

        return new UserProfileResponse(
           UserId: user.Id,
           FirstName: user.UserProfile.FirstName.Value,
           LastName: user.UserProfile.LastName.Value,
           AvatarUrl: user.UserProfile.AvatarUrl.Value,
           PhoneNumber: user.UserProfile.PhoneNumber.Value,
           Country: user.UserProfile.Location.Country.Name,
           City: user.UserProfile.Location.City);
    }
}
