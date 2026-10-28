using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options), IAppDbContext
    {
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Preserve existing domain entities while preventing EF Core
            // from attempting to map domain-event collections.
            builder.Entity<Project>().Ignore(x => x.DomainEvents);
            builder.Entity<TaskItem>().Ignore(x => x.DomainEvents);

            // Existing TaskFlow indexes — retained from Day 06.
            builder.Entity<Project>().HasIndex(p => p.TenantId);

            builder.Entity<TaskItem>().HasIndex(t => new { t.TenantId, t.ProjectId });
        }
    }
}