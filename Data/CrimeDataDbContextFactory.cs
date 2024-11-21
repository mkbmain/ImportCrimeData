using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class CrimeDataDbContextFactory : IDesignTimeDbContextFactory<CrimeDataDbContext>
{
    public CrimeDataDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        return new CrimeDataDbContext(BuildOptions(configuration.GetConnectionString("EventDb")!));
    }

    public static DbContextOptions<CrimeDataDbContext> BuildOptions(string connectionString)
        => new DbContextOptionsBuilder<CrimeDataDbContext>().UseSqlServer(connectionString).Options;
}