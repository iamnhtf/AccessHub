using AccessHub.API.Enums;

namespace AccessHub.API.Entities;

public class AccessRequest
{
    public Guid Id { get; set; }

    public string RequestCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public RequestStatus Status { get; set; }

    public string? RejectionReason { get; set; }

    public Guid RequestTypeId { get; set; }

    public RequestType RequestType { get; set; } = null!;

    public Guid RequestedBy { get; set; }

    public User Requester { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
