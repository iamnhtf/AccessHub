using System.Security.Claims;
using AccessHub.API.Data;
using AccessHub.API.DTOs.AccessRequests;
using AccessHub.API.Entities;
using AccessHub.API.Enums;
using AccessHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccessRequestsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public AccessRequestsController(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [Authorize(Roles = "Employee")]
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

    [Authorize(Roles = "Employee")]
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

    [Authorize(Roles = "Manager")]
    [HttpGet("pending")]
    public IActionResult GetPendingRequests()
    {
        var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (managerId is null)
        {
            return Unauthorized();
        }

        var requests = _context
            .AccessRequests.Where(x =>
                x.Status == RequestStatus.PendingApproval
                && x.Requester.ManagerId == Guid.Parse(managerId)
            )
            .Select(x => new PendingRequestDto
            {
                Id = x.Id,
                RequestCode = x.RequestCode,
                EmployeeName = x.Requester.FullName,
                Title = x.Title,
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt,
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return Ok(requests);
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, ApproveRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var request = await _context
            .AccessRequests.Include(x => x.Requester)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (request is null)
            return NotFound();

        if (request.Status != RequestStatus.PendingApproval)
        {
            return BadRequest("Request has already been processed.");
        }

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

        await _emailService.SendEmailAsync(
            request.Requester.Email,
            "Access Request Approved",
            $"""
            Hello {request.Requester.FullName},

            Your access request has been approved.

            Request Code: {request.RequestCode}
            Title: {request.Title}

            Status: Approved
            """
        );

        return Ok(new { message = "Request approved" });
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, RejectRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var request = await _context
            .AccessRequests.Include(x => x.Requester)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (request is null)
            return NotFound();

        if (request.Status != RequestStatus.PendingApproval)
        {
            return BadRequest("Request has already been processed.");
        }

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

        await _emailService.SendEmailAsync(
            request.Requester.Email,
            "Access Request Rejected",
            $"""
            Hello {request.Requester.FullName},

            Your access request has been rejected.

            Request Code: {request.RequestCode}
            Title: {request.Title}

            Reason:
            {dto.Reason}

            Status: Rejected
            """
        );

        return Ok(new { message = "Request rejected" });
    }
}
