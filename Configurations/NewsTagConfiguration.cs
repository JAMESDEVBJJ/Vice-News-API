using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViceNews.API.Models;

namespace ViceNews.API.Configurations;

public class NewsTagConfiguration : IEntityTypeConfiguration<NewsTag>
{
    public void Configure(EntityTypeBuilder<NewsTag> builder)
    {
        builder.ToTable("NewsTags");

        builder.HasKey(x => new { x.NewsId, x.TagId });

        builder.HasOne(x => x.News)
            .WithMany(n => n.NewsTags)
            .HasForeignKey(x => x.NewsId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(t => t.NewsTags)
            .HasForeignKey(x => x.TagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TagId);
    }
}
