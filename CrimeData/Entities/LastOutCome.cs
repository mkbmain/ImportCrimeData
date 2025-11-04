namespace CrimeData.Entities;

public sealed class LastOutCome : LookupTableBase<short>
{
    public ICollection<CrimeData> CrimeDatas { get; set; }
}