using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViceNews.API.Models;

namespace ViceNews.API.Configurations;

public class VehicleTagConfiguration : IEntityTypeConfiguration<VehicleTag>
{
    public void Configure(EntityTypeBuilder<VehicleTag> builder)
    {
        builder.ToTable("VehicleTags", t =>
            // Slug must be stored normalized (lowercase, trimmed), as in the logical model.
            t.HasCheckConstraint("CK_VehicleTags_Slug_Normalized", "\"Slug\" = lower(btrim(\"Slug\"))"));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityByDefaultColumn();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(120);

        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
