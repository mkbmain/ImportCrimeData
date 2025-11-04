namespace CrimeData.Entities;

public sealed class LSOAName: LookupTableBase<int>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}