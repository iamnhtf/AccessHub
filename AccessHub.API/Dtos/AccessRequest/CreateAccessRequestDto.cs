namespace AccessHub.API.DTOs.AccessRequests;

public class CreateAccessRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public Guid RequestTypeId { get; set; }
}
