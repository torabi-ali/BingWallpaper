using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Models.Configurations;

public class ImageInfoConfiguration : IEntityTypeConfiguration<ImageInfo>
{
    public void Configure(EntityTypeBuilder<ImageInfo> builder)
    {
        builder.Property(p => p.Name).IsRequired().HasMaxLength(16);
        builder.Property(p => p.Path).IsRequired().HasMaxLength(256);
        builder.Property(p => p.Headline).IsRequired().HasMaxLength(512);
        builder.Property(p => p.Url).IsRequired().HasMaxLength(1024);
        builder.Property(p => p.Copyright).IsRequired().HasMaxLength(2048);
        builder.HasIndex(p => new { p.Url });
    }
}