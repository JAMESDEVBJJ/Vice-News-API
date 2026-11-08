namespace ViceNews.API.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public long PriceGta { get; set; }
    public string Description { get; set; } = string.Empty;

    public int ManufacturerId { get; set; }
    public Manufacturer Manufacturer { get; set; } = null!;

    public int CategoryId { get; set; }
    public VehicleCategory Category { get; set; } = null!;

    public short MaxSpeed { get; set; }
    public short Acceleration { get; set; }
    public short Braking { get; set; }
    public short Handling { get; set; }

    public string? ImageUrl { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<VehicleTagAssignment> VehicleTagAssignments { get; set; } = new List<VehicleTagAssignment>();
}
