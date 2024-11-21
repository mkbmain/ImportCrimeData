using CrimeData.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrimeData;
public class CrimeDataDbContext : DbContext
{
    public CrimeDataDbContext(string connectionString) : base(GetOptions(connectionString))
    {
    }

    private static DbContextOptions<CrimeDataDbContext> GetOptions(string connectionString) => CrimeDataDbContextFactory.BuildOptions(connectionString);

    public CrimeDataDbContext(DbContextOptions<CrimeDataDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Entities.CrimeData> ImportCrimeData { get; set; }
    public virtual DbSet<CrimeType> CrimeType { get; set; }
    public virtual DbSet<Autherity> Authorities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.CrimeData>(w =>
        {
            w.HasOne(q => q.Location).WithMany(r => r.CrimeDatas)
                .HasForeignKey(q => q.LocationId);

            w.HasOne(q => q.Month).WithMany(q => q.CrimeDatas)
                .HasForeignKey(t => t.MonthId);

            w.HasOne(q => q.CrimeType).WithMany(q => q.CrimeDatas)
                .HasForeignKey(t => t.CrimeTypeId);

            w.HasOne(q => q.ReportedBy).WithMany(q => q.CrimeDatasReportedBy)
                .HasForeignKey(t => t.ReportedById);

            w.HasOne(q => q.Fallswithin).WithMany(q => q.CrimeDatasFallsWithIn)
                .HasForeignKey(t => t.FallswithinId);
        });
    }
}