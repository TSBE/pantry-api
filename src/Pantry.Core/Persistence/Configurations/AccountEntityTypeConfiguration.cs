using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence.Configurations;

public class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(column => column.AccountId);
        builder.Property(column => column.AccountId)
            .ValueGeneratedOnAdd();
        builder.Property(column => column.FirstName)
            .IsRequired();
        builder.Property(column => column.LastName)
            .IsRequired();
        builder.Property(column => column.FriendsCode)
            .IsRequired();
        builder.Property(column => column.HouseholdId);
        builder.Property(column => column.OAuhtId)
            .IsRequired();
        builder.HasIndex(b => b.OAuhtId)
            .IsUnique();
        builder.Property(column => column.CreatedAt)
            .IsRequired();
        builder.Property(column => column.UpdatedAt);
    }
}
