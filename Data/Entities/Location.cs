namespace Data.Entities;
public sealed class Location : LookupTableBase<int>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}