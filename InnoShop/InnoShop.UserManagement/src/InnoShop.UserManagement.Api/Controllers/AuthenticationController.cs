using ErrorOr;
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using InnoShop.UserManagement.Application.Authentication.Commands.Register;
using InnoShop.UserManagement.Application.Authentication.Common;
using InnoShop.UserManagement.Application.Authentication.Queries.Login;
using InnoShop.UserManagement.Contracts.Authentication;
using InnoShop.UserManagement.Application.Authentication.Commands.VerifyEmail;

namespace InnoShop.UserManagement.Api.Controllers;

[Route("[controller]")]
[AllowAnonymous]
public class AuthenticationController(ISender _mediator) : ApiController
{

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.Email, request.Password);
        ErrorOr<AuthenticationResult> authResult = await _mediator.Send(command);

        return authResult.Match(
            authResult => base.Ok(MapToAuthResponse(authResult)),
            Problem);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);
        var authResult = await _mediator.Send(query);

        if (authResult.IsError && authResult.FirstError == AuthenticationErrors.InvalidCredentials)
        {
            return Problem(
                detail: authResult.FirstError.Description,
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return authResult.Match(
            authResult => Ok(MapToAuthResponse(authResult)),
            Problem);
    }

    [HttpGet("verify-email", Name = "VerifyEmailRoute")]
    public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var command = new VerifyEmailCommand(userId, token);
        var result = await _mediator.Send(command);

        return result.Match(
            success => Ok("Email successfully verified!"),
            Problem);
    }

    private static AuthenticationResponse MapToAuthResponse(AuthenticationResult authResult)
    {
        return new AuthenticationResponse(
            authResult.User.Id,
            authResult.User.Email.Value,
            authResult.Token);
    }

}
