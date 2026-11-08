namespace ViceNews.API.Models;

public class VehicleCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
