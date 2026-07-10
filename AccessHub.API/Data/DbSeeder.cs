using AccessHub.API.Entities;

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
        }

        await context.SaveChangesAsync();
    }
}
