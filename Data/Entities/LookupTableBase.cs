namespace Data.Entities;
public abstract class LookupTableBase<T>
{
    public T Id { get; set; }
    public string Value { get; set; }
}