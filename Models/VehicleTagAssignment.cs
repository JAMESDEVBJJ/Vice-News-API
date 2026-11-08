namespace ViceNews.API.Models;

public class VehicleTagAssignment
{
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public int VehicleTagId { get; set; }
    public VehicleTag VehicleTag { get; set; } = null!;
}
