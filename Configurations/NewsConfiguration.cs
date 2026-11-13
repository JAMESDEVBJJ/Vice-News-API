using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViceNews.API.Models;

namespace ViceNews.API.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News", t =>
        {
            t.HasCheckConstraint("CK_News_ViewCount", "\"ViewCount\" >= 0");
            t.HasCheckConstraint("CK_News_ReadingTimeMinutes", "\"ReadingTimeMinutes\" >= 1");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityByDefaultColumn();

        builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Summary).IsRequired().HasColumnType("text");
        builder.Property(x => x.Content).IsRequired().HasColumnType("jsonb");
        builder.Property(x => x.HeroImageUrl).IsRequired().HasColumnType("text");

        builder.Property(x => x.Featured).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.ViewCount).IsRequired().HasColumnType("bigint").HasDefaultValue(0L);
        builder.Property(x => x.ReadingTimeMinutes).IsRequired();

        builder.Property(x => x.SidebarNoteLabel).HasColumnType("text");
        builder.Property(x => x.SidebarNoteText).HasColumnType("text");

        builder.Property(x => x.PublishedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasConversion<UtcDateTimeOffsetConverter>();

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

        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.AuthorId);
    }
}
