using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Yggdrasil.AuthServer.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Applications = Set<OpenIddictEntityFrameworkCoreApplication>();
        Authorizations = Set<OpenIddictEntityFrameworkCoreAuthorization>();
        Scopes = Set<OpenIddictEntityFrameworkCoreScope>();
        Tokens = Set<OpenIddictEntityFrameworkCoreToken>();
    }

    public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }
}
