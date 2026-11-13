using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViceNews.API.Models;

namespace ViceNews.API.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityByDefaultColumn();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Role).IsRequired().HasMaxLength(30).HasDefaultValue("Admin");
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()")
            .HasConversion<UtcDateTimeOffsetConverter>();

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()")
            .HasConversion<UtcDateTimeOffsetConverter>();

        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasMany(x => x.AuthoredNews)
            .WithOne(n => n.Author)
            .HasForeignKey(n => n.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
