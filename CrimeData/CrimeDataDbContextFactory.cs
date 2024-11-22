using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CrimeData;

public class CrimeDataDbContextFactory : IDesignTimeDbContextFactory<CrimeDataDbContext>
{
    public static string GetConnectionString()
    {
        var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        return configuration.GetConnectionString("Db")!;
    }
    public CrimeDataDbContext CreateDbContext(string[] args) => new(BuildOptions(GetConnectionString()));
    

    public static DbContextOptions<CrimeDataDbContext> BuildOptions(string connectionString)
        => new DbContextOptionsBuilder<CrimeDataDbContext>().UseSqlServer(connectionString).Options;
}