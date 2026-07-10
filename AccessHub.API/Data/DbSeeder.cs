using AccessHub.API.Entities;
using BCrypt.Net;

namespace AccessHub.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Id = Guid.NewGuid(), Name = "Admin" },
                new Role { Id = Guid.NewGuid(), Name = "Employee" },
                new Role { Id = Guid.NewGuid(), Name = "Manager" },
                new Role { Id = Guid.NewGuid(), Name = "ITSupport" }
            );

            await context.SaveChangesAsync();
        }

        if (!context.RequestTypes.Any())
        {
            context.RequestTypes.AddRange(
                new RequestType { Id = Guid.NewGuid(), Name = "AWS Account" },
                new RequestType { Id = Guid.NewGuid(), Name = "GitHub Access" },
                new RequestType { Id = Guid.NewGuid(), Name = "VPN Access" },
                new RequestType { Id = Guid.NewGuid(), Name = "Database Access" },
                new RequestType { Id = Guid.NewGuid(), Name = "Jira Access" },
                new RequestType { Id = Guid.NewGuid(), Name = "Confluence Access" },
                new RequestType { Id = Guid.NewGuid(), Name = "Software Installation" }
            );

            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var adminRole = context.Roles.First(x => x.Name == "Admin");
            var managerRole = context.Roles.First(x => x.Name == "Manager");
            var employeeRole = context.Roles.First(x => x.Name == "Employee");

            var managerId = Guid.NewGuid();

            context.Users.AddRange(
                new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "System Admin",
                    Email = "admin@accesshub.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RoleId = adminRole.Id,
                },
                new User
                {
                    Id = managerId,
                    FullName = "Project Manager",
                    Email = "manager@accesshub.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RoleId = managerRole.Id,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Test Employee",
                    Email = "employee@accesshub.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RoleId = employeeRole.Id,
                    ManagerId = managerId,
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
