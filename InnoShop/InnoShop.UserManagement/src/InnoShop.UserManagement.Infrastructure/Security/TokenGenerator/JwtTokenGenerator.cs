using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InnoShop.UserManagement.Application.Common.Interfaces;
using InnoShop.UserManagement.Domain.UserAggregate;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InnoShop.UserManagement.Infrastructure.Security.TokenGenerator;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim("id", user.Id.ToString())
        };

        // Add roles
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        // Add permissions (flattened from all roles)
        var permissions = user.Roles
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .Distinct();

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission));
        }

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}