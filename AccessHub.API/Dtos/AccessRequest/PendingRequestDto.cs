namespace AccessHub.API.DTOs.AccessRequests;

public class PendingRequestDto
{
    public Guid Id { get; set; }

    public string RequestCode { get; set; } = string.Empty;

    public string EmployeeName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
