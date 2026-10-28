using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Infrastructure.Identity;

public sealed class AppUser : IdentityUser<Guid>
{
    public Guid TenantId { get; set; }
}