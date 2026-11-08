using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViceNews.API.Models;

namespace ViceNews.API.Configurations;

public class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
{
    public void Configure(EntityTypeBuilder<Manufacturer> builder)
    {
        builder.ToTable("Manufacturers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityByDefaultColumn();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(120);

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Slug).IsUnique();

        builder.HasMany(x => x.Vehicles)
            .WithOne(v => v.Manufacturer)
            .HasForeignKey(v => v.ManufacturerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
