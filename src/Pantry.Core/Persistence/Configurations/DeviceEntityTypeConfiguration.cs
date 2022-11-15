using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence.Configurations;

public class DeviceEntityTypeConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(column => column.DeviceId);
        builder.Property(column => column.DeviceId)
            .ValueGeneratedOnAdd();
        builder.Property(column => column.Name)
            .IsRequired();
        builder.Property(column => column.Model)
            .IsRequired();
        builder.HasIndex(b => b.DeviceToken)
            .IsUnique();
        builder.Property(column => column.Platform)
            .IsRequired();
        builder.HasIndex(b => b.InstallationId)
            .IsUnique();
        builder.Property(column => column.CreatedAt)
            .IsRequired();
        builder.Property(column => column.UpdatedAt);
    }
}
