using System.Collections.Immutable;
using CrimeData;
using CrimeData.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

public class Program
{
    private static CrimeDataDbContext GetNewDbContext() => new(CrimeDataDbContextFactory.GetConnectionString());

    private static CrimeDataDbContext _context = GetNewDbContext();

    public static async Task Main(string[] args)
    {
        await _context.Database.MigrateAsync();
        if (args.Length < 2)
        {
            Console.WriteLine("A path must be specified.");
            Console.WriteLine("Options:");
            Console.WriteLine("    ImportCrimeData {FolderPath}");
            Console.WriteLine("    ImportPostCodes {FilePath}");
        }

        switch (args[0].ToLower())
        {
            case "importcrimedata":
                if (!Directory.Exists(args[1]))
                {
                    Console.WriteLine("The folder path does not exist.");
                    return;
                }

                await ImportCrimeData(args[1]);
                return;
            case "importpostcodes":
                if (!File.Exists(args[1]))
                {
                    Console.WriteLine("The File path does not exist.");
                    return;
                }

                await ImportPostCodes(args[1]);
                break;
        }
    }

    private static async Task ImportPostCodes(string filePath)
    {
        var parts = File.ReadLines(filePath)
            .Select(w => w.Split(",").ToList());

        var lonLoc = parts.First().IndexOf(parts.First()
            .FirstOrDefault(q => q.Contains("long", StringComparison.CurrentCultureIgnoreCase)));
        var LatLoc = parts.First().IndexOf(parts.First()
            .FirstOrDefault(q => q.Contains("lat", StringComparison.CurrentCultureIgnoreCase)));
        var parseAll = parts.Skip(1)
            .Select(e => new
            {
                PostCode = e[0].Trim().ToLower().Replace(" ", "").Replace("\"",""),
                LatitudeStr = e[LatLoc].Trim(),
                LongitudeStr = e[lonLoc].Trim()
            }).Where(e => decimal.TryParse(e.LatitudeStr, out _) && decimal.TryParse(e.LongitudeStr, out _)).ToArray();

        var total = 0;
        foreach (var item in parseAll.Chunk(5000))
        {
            total += item.Length;
            var codes = item.Select(e => e.PostCode).ToArray();
            var existing = await _context.PostCodes.Where(w => codes.Contains(w.Code)).Select(w => w.Code)
                .ToArrayAsync();
            var lookup = existing.ToHashSet();
            var block = item.Where(w => !lookup.Contains(w.PostCode))
                .Select(w => new PostCode
                {
                    Code = w.PostCode,
                    Longitude = decimal.Parse(w.LongitudeStr),
                    Latitude = decimal.Parse(w.LatitudeStr),
                }).ToArray();
            await _context.BulkInsertAsync(block);

            Console.WriteLine($"Imported {total} postcodes out of {parseAll.Length}");
        }
    }

    private static async Task ImportCrimeData(string folderPath)
    {
        var files = Directory.GetFiles(folderPath, "*street.csv", SearchOption.AllDirectories).OrderBy(w => w)
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
                    LSOAcode = w[7] ?? string.Empty,
                    LSOAname = w[8] ?? string.Empty,
                    LastOutcome = w[10],
                    Context = string.IsNullOrWhiteSpace(w[11]) ? null : w[11]
                }).ToArray();

            var crimeTypes = await _context.PopulateShort<CrimeType>(data.Select(w => w.CrimeType).Distinct());
            var month = await _context.PopulateShort<Month>(data.Select(w => w.Month).Distinct());
            var authorities = await _context.PopulateShort<Autherity>(data.Select(w => w.Fallswithin)
                .Union(data.Select(q => q.ReportedBy)).Distinct());
            var locations = await _context.PopulateInt<Location>(data.Select(w => w.Location).Distinct());
            
            var LsoaNames = await _context.PopulateInt<LSOAName>(data.Select(w => w.LSOAname).Distinct());
            var LsoaCodes = await _context.PopulateInt<LSOAcode>(data.Select(w => w.LSOAcode).Distinct());
            foreach (var batch in data.Select(item => new CrimeData.Entities.CrimeData()
                     {
                         CrimeId = item.CrimeID,
                         CrimeTypeId = crimeTypes[item.CrimeType],
                         MonthId = month[item.Month],
                         ReportedById = authorities[item.ReportedBy],
                         FallswithinId = authorities[item.Fallswithin],
                         Longitude = item.Longitude,
                         Latitude = item.Latitude,
                         LSOAcodeId = LsoaCodes[item.LSOAcode],
                         LSOAnameId = LsoaNames[item.LSOAname],
                         LastOutcome = item.LastOutcome,
                         LocationId = locations[item.Location],
                         Context = item.Context
                     }).Chunk(1000))
            {
                await _context.BulkInsertAsync(batch);
            }

            File.Delete(file);
            await _context.DisposeAsync();
            _context = GetNewDbContext();
        }
    }
}