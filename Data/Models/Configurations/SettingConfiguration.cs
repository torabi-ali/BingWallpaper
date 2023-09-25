using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Models.Configurations;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(p => p.Key);

        builder.Property(p => p.Key).IsRequired().HasMaxLength(256);
        builder.Property(p => p.Value).IsRequired().HasMaxLength(512);
    }
}