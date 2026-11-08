using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViceNews.API.Models;

namespace ViceNews.API.Configurations;

public class VehicleCategoryConfiguration : IEntityTypeConfiguration<VehicleCategory>
{
    public void Configure(EntityTypeBuilder<VehicleCategory> builder)
    {
        builder.ToTable("VehicleCategories");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityByDefaultColumn();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(120);

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Slug).IsUnique();

        builder.HasMany(x => x.Vehicles)
            .WithOne(v => v.Category)
            .HasForeignKey(v => v.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
