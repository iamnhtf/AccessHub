using System.Security.Claims;
using AccessHub.API.Data;
using AccessHub.API.DTOs.AccessRequests;
using AccessHub.API.Entities;
using AccessHub.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccessRequestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccessRequestsController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccessRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var request = new AccessRequest
        {
            Id = Guid.NewGuid(),
            RequestCode = $"REQ-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Title = dto.Title,
            Reason = dto.Reason,
            RequestTypeId = dto.RequestTypeId,
            RequestedBy = Guid.Parse(userId),

            Status = RequestStatus.PendingApproval,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.AccessRequests.Add(request);

        await _context.SaveChangesAsync();

        return Ok(
            new
            {
                request.Id,
                request.RequestCode,
                request.Title,
                request.Status,
            }
        );
    }

    [Authorize]
    [HttpGet("my")]
    public IActionResult GetMyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var requests = _context
            .AccessRequests.Where(x => x.RequestedBy == Guid.Parse(userId))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AccessRequestResponseDto
            {
                Id = x.Id,
                RequestCode = x.RequestCode,
                Title = x.Title,
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt,
            })
            .ToList();

        return Ok(requests);
    }
}
