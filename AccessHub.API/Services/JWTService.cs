using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AccessHub.API.Entities;
using Microsoft.IdentityModel.Tokens;

namespace AccessHub.API.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;
    private readonly ParameterStoreService _parameterStoreService;

    public JwtService(IConfiguration configuration, ParameterStoreService parameterStoreService)
    {
        _configuration = configuration;
        _parameterStoreService = parameterStoreService;
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name),
        };

        var jwtKey = await _parameterStoreService.GetParameterAsync("/accesshub/jwt-key");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
