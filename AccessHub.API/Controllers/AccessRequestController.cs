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

    [Authorize]
    [HttpGet("pending")]
    public IActionResult GetPendingRequests()
    {
        var requests = _context
            .AccessRequests.Where(x => x.Status == RequestStatus.PendingApproval)
            .Join(
                _context.Users,
                request => request.RequestedBy,
                user => user.Id,
                (request, user) =>
                    new PendingRequestDto
                    {
                        Id = request.Id,
                        RequestCode = request.RequestCode,
                        EmployeeName = user.FullName,
                        Title = request.Title,
                        Status = request.Status.ToString(),
                        CreatedAt = request.CreatedAt,
                    }
            )
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return Ok(requests);
    }

    [Authorize]
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, ApproveRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var request = await _context.AccessRequests.FindAsync(id);

        if (request is null)
            return NotFound();

        request.Status = RequestStatus.Approved;
        request.UpdatedAt = DateTime.UtcNow;

        var approval = new RequestApproval
        {
            Id = Guid.NewGuid(),
            RequestId = request.Id,
            ApproverId = Guid.Parse(userId),
            Decision = "Approved",
            Comment = dto.Comment,
            DecisionAt = DateTime.UtcNow,
        };

        _context.RequestApprovals.Add(approval);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Request approved" });
    }

    [Authorize]
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, RejectRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var request = await _context.AccessRequests.FindAsync(id);

        if (request is null)
            return NotFound();

        request.Status = RequestStatus.Rejected;
        request.RejectionReason = dto.Reason;
        request.UpdatedAt = DateTime.UtcNow;

        var approval = new RequestApproval
        {
            Id = Guid.NewGuid(),
            RequestId = request.Id,
            ApproverId = Guid.Parse(userId),
            Decision = "Rejected",
            Comment = dto.Reason,
            DecisionAt = DateTime.UtcNow,
        };

        _context.RequestApprovals.Add(approval);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Request rejected" });
    }
}
