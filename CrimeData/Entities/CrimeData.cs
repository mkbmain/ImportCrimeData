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
    public int? LSOAcodeId { get; set; }
    public int? LSOAnameId { get; set; }
    public short? CrimeTypeId { get; set; }
    public short? LastOutcomeId { get; set; }
    public string? Context { get; set; }

    public virtual Month Month { get; set; }
    public virtual CrimeType CrimeType { get; set; }

    public virtual Location Location { get; set; }
    public virtual Autherity ReportedBy { get; set; }
    public virtual Autherity Fallswithin { get; set; }
    public virtual LSOAcode LsoAcode { get; set; }
    public virtual LSOAName LSOAName { get; set; }
    public virtual LastOutCome LastOutCome { get; set; }
}