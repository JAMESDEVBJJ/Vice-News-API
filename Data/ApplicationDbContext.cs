using Microsoft.EntityFrameworkCore;
using ViceNews.API.Models;

namespace ViceNews.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<VehicleCategory> VehicleCategories => Set<VehicleCategory>();
    public DbSet<VehicleTag> VehicleTags => Set<VehicleTag>();
    public DbSet<VehicleTagAssignment> VehicleTagAssignments => Set<VehicleTagAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
