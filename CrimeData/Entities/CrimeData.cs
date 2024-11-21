namespace CrimeData.Entities;
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