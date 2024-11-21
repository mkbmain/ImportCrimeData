namespace Data.Entities;
public sealed class Month : LookupTableBase<short>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}