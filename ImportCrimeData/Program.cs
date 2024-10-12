using System.Collections.Immutable;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

public class Program
{
    private const string FolderPath = "/home/mkb/Downloads/CrimeData/";

    public const string ConnectionString =
        "Server=localhost;Database=CrimeData;User Id=sa;Password=A1234567a;TrustServerCertificate=True";

    public static async Task Main()
    {
        var db = new ExampleDbContext();
        await db.Database.MigrateAsync();
        var files = Directory.GetFiles(FolderPath, "*street.csv", SearchOption.AllDirectories).OrderBy(w => w)
            .ToImmutableArray();
        foreach (var file in files)
        {
            Console.WriteLine($"Processing{file}");
            var data = File.ReadLines(file)
                .Skip(1)
                .Select(q => q.Split(','))
                .Select(w => new
                {
                    Month = w[1],
                    CrimeType = w[9],
                    CrimeID = w[0],
                    ReportedBy = w[2],
                    Fallswithin = w[3],
                    Longitude = string.IsNullOrWhiteSpace(w[4]) ? (decimal?)null : decimal.Parse(w[4]),
                    Latitude = string.IsNullOrWhiteSpace(w[5]) ? (decimal?)null : decimal.Parse(w[5]),
                    Location = w[6],
                    LSOAcode = w[7],
                    LSOAname = w[8],
                    LastOutcome = w[10],
                    Context = string.IsNullOrWhiteSpace(w[11]) ? null : w[11]
                }).ToArray();

            var crimeTypes = await db.PopulateShort<CrimeType>(data.Select(w => w.CrimeType).Distinct());
            var month = await db.PopulateShort<Month>(data.Select(w => w.Month).Distinct());
            var authorities = await db.PopulateShort<Autherity>(data.Select(w => w.Fallswithin)
                .Union(data.Select(q => q.ReportedBy)).Distinct());
            var locations = await db.PopulateInt<Location>(data.Select(w => w.Location).Distinct());
            foreach (var batch in data.Select(item => new CrimeData()
                     {
                         CrimeId = item.CrimeID,
                         CrimeTypeId = crimeTypes[item.CrimeType],
                         MonthId = month[item.Month],
                         ReportedById = authorities[item.ReportedBy],
                         FallswithinId = authorities[item.Fallswithin],
                         Longitude = item.Longitude,
                         Latitude = item.Latitude,
                         LSOAcode = item.LSOAcode,
                         LSOAname = item.LSOAname,
                         LastOutcome = item.LastOutcome,
                         LocationId = locations[item.Location],
                         Context = item.Context
                     }).Chunk(1000))
            {
                await db.BulkInsertAsync(batch);
            }

            File.Delete(file);
            await db.DisposeAsync();
            db = new ExampleDbContext();
        }
    }
}

public class ExampleDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        optionsBuilder.UseSqlServer(Program.ConnectionString);
    }

    public virtual DbSet<CrimeData> ImportCrimeData { get; set; }
    public virtual DbSet<CrimeType> CrimeType { get; set; }
    public virtual DbSet<Autherity> Authorities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CrimeData>(w =>
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

public class CrimeData
{
    public int Id { get; set; }
    public string? CrimeId { get; set; }
    public short? MonthId { get; set; }
    public short? ReportedById { get; set; }

    public short? FallswithinId { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public int? LocationId { get; set; }
    public string? LSOAcode { get; set; }
    public string? LSOAname { get; set; }
    public short? CrimeTypeId { get; set; }
    public string? LastOutcome { get; set; }
    public string? Context { get; set; }

    public virtual Month Month { get; set; }
    public virtual CrimeType CrimeType { get; set; }

    public virtual Location Location { get; set; }
    public virtual Autherity ReportedBy { get; set; }
    public virtual Autherity Fallswithin { get; set; }
}

public sealed class Location : LookupTable<int>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}

public sealed class Autherity : LookupTable<short>
{
    public ICollection<CrimeData> CrimeDatasReportedBy { get; set; }
    public ICollection<CrimeData> CrimeDatasFallsWithIn { get; set; }
}

public sealed class Month : LookupTable<short>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}

public sealed class CrimeType : LookupTable<short>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}

public abstract class LookupTable<T>
{
    public T Id { get; set; }
    public string Value { get; set; }
}

public static class Extensions
{
    public static async Task<Dictionary<string, short>> PopulateShort<TDbModel>(this DbContext dbContext,
        IEnumerable<string> items)
        where TDbModel : LookupTable<short>, new() => await dbContext.Populate<TDbModel, short>(items);

    public static async Task<Dictionary<string, int>> PopulateInt<TDbModel>(this DbContext dbContext,
        IEnumerable<string> items)
        where TDbModel : LookupTable<int>, new() => await dbContext.Populate<TDbModel, int>(items);

    public static async Task<Dictionary<string, TLookupType>> Populate<TDbModel, TLookupType>(this DbContext db,
        IEnumerable<string> items, int count = 1)
        where TDbModel : LookupTable<TLookupType>, new()
    {
        var output = await db.Set<TDbModel>().Where(w => items.Contains(w.Value))
            .GroupBy(w => w.Value)
            .ToDictionaryAsync(w => w.Key, w => w.First().Id);

        var all = items.Where(w => !output.ContainsKey(w)).Select(q => new TDbModel { Value = q }).ToArray();
        if (all.Any() && count < 2)
        {
            await db.BulkInsertAsync(all);
           
            return await db.Populate<TDbModel, TLookupType>(items, count + 1);
        }


        return output;
    }
}