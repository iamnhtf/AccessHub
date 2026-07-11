namespace AccessHub.API.DTOs.Users;

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Guid RoleId { get; set; }
    public Guid? ManagerId { get; set; }
}
