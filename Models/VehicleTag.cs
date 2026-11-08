namespace ViceNews.API.Models;

public class VehicleTag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<VehicleTagAssignment> VehicleTagAssignments { get; set; } = new List<VehicleTagAssignment>();
}
