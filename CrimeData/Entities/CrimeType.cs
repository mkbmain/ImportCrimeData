namespace CrimeData.Entities;
public sealed class CrimeType : LookupTableBase<short>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}