using System.Security.Claims;
using AccessHub.API.Data;
using AccessHub.API.DTOs.Auth;
using AccessHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context
            .Users.Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }

        if (!user.IsActive)
        {
            return Unauthorized("Account has been deactivated");
        }

        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            return Unauthorized("Invalid email or password");
        }

        var token = await _jwtService.GenerateTokenAsync(user);

        return Ok(new { token, mustChangePassword = user.MustChangePassword });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(Guid.Parse(userId));

        if (user is null)
        {
            return NotFound();
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
        {
            return BadRequest("Current password is incorrect");
        }

        bool valid =
            dto.NewPassword.Length >= 8
            && dto.NewPassword.Any(char.IsUpper)
            && dto.NewPassword.Any(char.IsLower)
            && dto.NewPassword.Any(char.IsDigit);

        if (!valid)
        {
            return BadRequest(
                "Password must contain uppercase, lowercase, number and be at least 8 characters."
            );
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.MustChangePassword = false;

        await _context.SaveChangesAsync();

        return Ok("Password changed successfully");
    }
}
