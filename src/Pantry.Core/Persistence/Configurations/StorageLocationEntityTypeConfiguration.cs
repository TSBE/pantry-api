using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence.Configurations;

public class StorageLocationEntityTypeConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.HasKey(column => column.StorageLocationId);
        builder.Property(column => column.StorageLocationId)
            .ValueGeneratedOnAdd();
        builder.Property(column => column.Name)
            .IsRequired();
        builder.Property(column => column.Description);
        builder.Property(column => column.HouseholdId)
            .IsRequired();
        builder.Property(column => column.CreatedAt)
            .IsRequired();
        builder.Property(column => column.UpdatedAt);
    }
}
