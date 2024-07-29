using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Context;
public class BaseDbContext : DbContext 
{
    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Log> Logs { get; set; } 
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
}