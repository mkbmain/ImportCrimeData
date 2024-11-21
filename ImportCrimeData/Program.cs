using System.Collections.Immutable;
using Data;
using Data.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

public class Program
{
    private const string FolderPath = "/home/mkb/Downloads/CrimeData/";

    public const string ConnectionString =
        "Server=localhost;Database=CrimeData;User Id=sa;Password=A1234567a;TrustServerCertificate=True";

    private static CrimeDataDbContext GetNewDbContext() => new CrimeDataDbContext(ConnectionString);

    public static async Task Main()
    {
        var db = GetNewDbContext();
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
            db = GetNewDbContext();
        }
    }
}