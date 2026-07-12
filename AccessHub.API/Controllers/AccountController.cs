using AccessHub.API.Data;
using AccessHub.API.DTOs.Users;
using AccessHub.API.Entities;
using AccessHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    private readonly ILogger<AccountController> _logger;

    public AccountController(
        AppDbContext context,
        EmailService emailService,
        ILogger<AccountController> logger
    )
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var existingUser = _context.Users.Any(x => x.Email == dto.Email);

        if (existingUser)
        {
            return BadRequest("Email already exists");
        }

        var role = await _context.Roles.FindAsync(dto.RoleId);

        if (role is null)
        {
            return BadRequest("Role not found");
        }

        const string temporaryPassword = "Temp@123";

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
            RoleId = dto.RoleId,
            ManagerId = dto.ManagerId,
            IsActive = true,
            MustChangePassword = true,
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        try
        {
            await _emailService.SendEmailAsync(
                dto.Email,
                "AccessHub Account Created",
                $"""
                Welcome to AccessHub

                Email: {dto.Email}
                Password: {temporaryPassword}
                Please change your password after first login.
                """
            );
        }
        catch (Exception ex)
        {
            //Ignore email errors in development
        }

        _logger.LogInformation("User {Email} created with role {RoleId}", dto.Email, dto.RoleId);

        return Ok(new { Message = "User created successfully. Credentials sent by email" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context
            .Users.Include(x => x.Role)
            .Select(x => new
            {
                x.Id,
                x.FullName,
                x.Email,
                Role = x.Role.Name,
                x.ManagerId,
                x.IsActive,
            })
            .ToListAsync();

        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound("User not found");
        }

        user.IsActive = false;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {Email} deactivated", user.Email);

        return Ok(new { Message = "User deactivated successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound("User not found");
        }

        const string temporaryPassword = "Temp@123";

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);

        user.MustChangePassword = true;

        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(
            user.Email,
            "AccessHub Password Reset",
            $"""
            Your password has been reset.

            Email: {user.Email}
            Password: {temporaryPassword}

            Please change your password after login.
            """
        );

        _logger.LogWarning("Password reset requested for {Email}", user.Email);

        return Ok(new { Message = "Password reset successfully" });
    }
}
