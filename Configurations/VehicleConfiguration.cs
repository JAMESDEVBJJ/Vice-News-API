using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ViceNews.API.Models;

namespace ViceNews.API.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    // Npgsql only accepts UTC offsets for timestamptz; normalize on write.
    private static readonly ValueConverter<DateTimeOffset, DateTimeOffset> UtcConverter =
        new(v => v.ToUniversalTime(), v => v.ToUniversalTime());

    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles", t =>
        {
            t.HasCheckConstraint("CK_Vehicles_PriceGta", "\"PriceGta\" >= 0");
            t.HasCheckConstraint("CK_Vehicles_MaxSpeed", "\"MaxSpeed\" BETWEEN 0 AND 100");
            t.HasCheckConstraint("CK_Vehicles_Acceleration", "\"Acceleration\" BETWEEN 0 AND 100");
            t.HasCheckConstraint("CK_Vehicles_Braking", "\"Braking\" BETWEEN 0 AND 100");
            t.HasCheckConstraint("CK_Vehicles_Handling", "\"Handling\" BETWEEN 0 AND 100");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityByDefaultColumn();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(180);
        builder.Property(x => x.PriceGta).IsRequired().HasColumnType("bigint");
        builder.Property(x => x.Description).IsRequired().HasColumnType("text");

        builder.Property(x => x.MaxSpeed).IsRequired().HasColumnType("smallint");
        builder.Property(x => x.Acceleration).IsRequired().HasColumnType("smallint");
        builder.Property(x => x.Braking).IsRequired().HasColumnType("smallint");
        builder.Property(x => x.Handling).IsRequired().HasColumnType("smallint");

        builder.Property(x => x.ImageUrl).HasColumnType("text");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()")
            .HasConversion(UtcConverter);

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()")
            .HasConversion(UtcConverter);

        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.ManufacturerId);
        builder.HasIndex(x => x.CategoryId);
    }
}
