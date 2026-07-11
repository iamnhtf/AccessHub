namespace AccessHub.API.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;

        public Guid? ManagerId { get; set; }

        public User? Manager { get; set; }

        public ICollection<AccessRequest> Requests { get; set; } = [];

        public ICollection<RequestApproval> Approvals { get; set; } = [];

        public bool IsActive { get; set; } = true;

        public bool MustChangePassword { get; set; } = true;
    }
}
