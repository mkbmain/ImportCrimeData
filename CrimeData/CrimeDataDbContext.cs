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
    public virtual DbSet<LSOAcode> LSOAcode { get; set; }
    public virtual DbSet<LSOAName> LSOAName { get; set; }
    public virtual DbSet<PostCode> PostCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostCode>(entity =>
        {
            entity.HasIndex(w => w.Code).IsUnique();
            entity.Property(w => w.Longitude).HasPrecision(12, 7);
            entity.Property(w => w.Latitude).HasPrecision(12, 7);
        });

        modelBuilder.Entity<Entities.CrimeData>(w =>
        {
            w.HasOne(w => w.LsoAcode).WithMany(w => w.CrimeDatas)
                .HasForeignKey(q => q.LSOAcodeId);

            w.HasOne(w => w.LSOAName).WithMany(w => w.CrimeDatas)
                .HasForeignKey(q => q.LSOAnameId);

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