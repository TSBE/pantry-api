using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence.Configurations;

public class HouseholdEntityTypeConfiguration : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.HasKey(column => column.HouseholdId);
        builder.Property(column => column.HouseholdId)
            .ValueGeneratedOnAdd();
        builder.Property(column => column.Name)
            .IsRequired();
        builder.Property(column => column.OwnerId)
            .IsRequired();
        builder.Property(column => column.SubscriptionType)
            .IsRequired();
        builder.Property(column => column.CreatedAt)
            .IsRequired();
        builder.Property(column => column.UpdatedAt);
    }
}
