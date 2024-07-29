using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data.Context;
public class BaseDbContextFactory : IDesignTimeDbContextFactory<BaseDbContext>
{
    public BaseDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? throw new Exception("DB_HOST not found");
        var name = Environment.GetEnvironmentVariable("DB_NAME") ?? throw new Exception("DB_NAME not found");
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? throw new Exception("DB_USER not found");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new Exception("DB_PASSWORD not found");
        var connectionString = $"Server={host};Database={name};Uid={user};Pwd={password};SslMode=None;ConnectionTimeout=0";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new BaseDbContext(optionsBuilder.Options);
    }
}