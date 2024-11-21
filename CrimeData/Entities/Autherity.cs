namespace CrimeData.Entities;
public sealed class Autherity : LookupTableBase<short>
{
    public ICollection<CrimeData> CrimeDatasReportedBy { get; set; }
    public ICollection<CrimeData> CrimeDatasFallsWithIn { get; set; }
}