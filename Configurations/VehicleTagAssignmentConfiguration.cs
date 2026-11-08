using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViceNews.API.Models;

namespace ViceNews.API.Configurations;

public class VehicleTagAssignmentConfiguration : IEntityTypeConfiguration<VehicleTagAssignment>
{
    public void Configure(EntityTypeBuilder<VehicleTagAssignment> builder)
    {
        builder.ToTable("VehicleTagAssignments");

        builder.HasKey(x => new { x.VehicleId, x.VehicleTagId });

        builder.HasOne(x => x.Vehicle)
            .WithMany(v => v.VehicleTagAssignments)
            .HasForeignKey(x => x.VehicleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.VehicleTag)
            .WithMany(t => t.VehicleTagAssignments)
            .HasForeignKey(x => x.VehicleTagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VehicleTagId);
    }
}
