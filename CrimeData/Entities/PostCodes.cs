namespace CrimeData.Entities;

public class PostCode
{
    public int Id { get; set; }
    public string Code { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
}