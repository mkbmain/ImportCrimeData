using System.Diagnostics;
using CrimeData;
using CrimeData.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebAppViewer.Models;
using SimpleWebAppViewer.Utils;

namespace SimpleWebAppViewer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CrimeDataDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, CrimeDataDbContext dbContext)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet("Get/{postCode}")]
    public async Task<IActionResult> Post([FromRoute] string postCode, [FromQuery] double radiusInMiles)
    {
        radiusInMiles = radiusInMiles < 1 ? 1 : radiusInMiles;
        postCode = postCode?.ToLower().Trim();
        if (string.IsNullOrWhiteSpace(postCode))
        {
            return BadRequest();
        }

        var details = await _dbContext.PostCodes.FirstOrDefaultAsync(w => w.Code == postCode);
        if (details == null)
        {
            return BadRequest();
        }

        var bounds = GeoUtils.CalculateBounds((double)details.Latitude, (double)details.Longitude, radiusInMiles);

        var crimes = await _dbContext.ImportCrimeData.Where(w =>
                w.Longitude > (decimal)bounds.MinLongitude && w.Longitude < (decimal)bounds.MaxLongitude)
            .Where(w => w.Latitude > (decimal)bounds.MinLatitude && w.Latitude < (decimal)bounds.MaxLatitude)
            .OrderByDescending(w => w.Month.Value)
            .Select(w => new
            {
                w.Longitude,
                w.Latitude,
                Fallswithin = w.Fallswithin.Value,
                Month = w.Month.Value,
                Location = w.Location.Value,
                CrimeType = w.CrimeType.Value
            }).ToArrayAsync();
        var breakDown = crimes.GroupBy(w => w.CrimeType)
            .ToDictionary(w => w.Key, w => Math.Round(((decimal)w.Count() / (decimal)crimes.Length) * 100, 2));
 

        return Ok(new
        {
            BreakDown = breakDown,
            Raw = string.Join("<br>", breakDown.Select(w => $"{w.Key}: {w.Value}")),
            Data = crimes
        });
    }
}