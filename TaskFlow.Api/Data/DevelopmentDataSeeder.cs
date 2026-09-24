using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Data;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (await dbContext.Projects.AnyAsync())
        {
            return;
        }

        var tenantId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

        var project = new Project
        {
            Id = Guid.Parse(
                "22222222-2222-2222-2222-222222222222"),

            TenantId = tenantId,

            Name = "TaskFlow Development Project",

            Description = "Development project for local testing."
        };

        dbContext.Projects.Add(project);

        await dbContext.SaveChangesAsync();
    }
}