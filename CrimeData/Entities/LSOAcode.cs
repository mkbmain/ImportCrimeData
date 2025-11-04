namespace CrimeData.Entities;

public sealed class LSOAcode : LookupTableBase<int>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}