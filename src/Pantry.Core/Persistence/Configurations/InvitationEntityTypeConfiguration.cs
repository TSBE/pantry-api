using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence.Configurations;

public class InvitationEntityTypeConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasKey(column => column.InvitationId);
        builder.Property(column => column.InvitationId)
            .ValueGeneratedOnAdd();
        builder.Property(column => column.ValidUntilDate)
            .IsRequired();
        builder.Property(column => column.FriendsCode)
            .IsRequired();
        builder.HasIndex(b => b.FriendsCode);
        builder.Property(column => column.CreatorId)
            .IsRequired();
        builder.Property(column => column.HouseholdId)
            .IsRequired();
        builder.Property(column => column.CreatedAt)
            .IsRequired();
        builder.Property(column => column.UpdatedAt);
    }
}
