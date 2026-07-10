using AccessHub.API.Enums;

namespace AccessHub.API.Entities
{
    public class RequestApproval
    {
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }

        public AccessRequest Request { get; set; } = null!;

        public Guid ApproverId { get; set; }

        public User Approver { get; set; } = null!;

        public string Decision { get; set; } = string.Empty;

        public string? Comment { get; set; }

        public DateTime DecisionAt { get; set; }
    }
}
