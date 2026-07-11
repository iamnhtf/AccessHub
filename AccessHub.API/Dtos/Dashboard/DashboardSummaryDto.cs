namespace AccessHub.API.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalUsers { get; set; }

    public int ActiveUsers { get; set; }

    public int InactiveUsers { get; set; }

    public int PendingRequests { get; set; }

    public int ApprovedRequests { get; set; }

    public int RejectedRequests { get; set; }

    public int TotalAmins { get; set; }

    public int TotalManagers { get; set; }

    public int TotalEmployees { get; set; }
}
