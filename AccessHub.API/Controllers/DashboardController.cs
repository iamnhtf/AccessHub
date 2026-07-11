using AccessHub.API.Data;
using AccessHub.API.DTOs.Dashboard;
using AccessHub.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetDashboard()
    {
        var dashboard = new DashboardSummaryDto
        {
            TotalUsers = _context.Users.Count(),

            ActiveUsers = _context.Users.Count(x => x.IsActive),

            InactiveUsers = _context.Users.Count(x => !x.IsActive),

            PendingRequests = _context.AccessRequests.Count(x =>
                x.Status == RequestStatus.PendingApproval
            ),

            ApprovedRequests = _context.AccessRequests.Count(x =>
                x.Status == RequestStatus.Approved
            ),

            RejectedRequests = _context.AccessRequests.Count(x =>
                x.Status == RequestStatus.Rejected
            ),

            TotalAmins = _context.Users.Count(x => x.Role.Name == "Admin"),
            TotalManagers = _context.Users.Count(x => x.Role.Name == "Manager"),
            TotalEmployees = _context.Users.Count(x => x.Role.Name == "Employee"),
        };

        return Ok(dashboard);
    }
}
