using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence.Configurations;

public class MetadataleEntityTypeConfiguration : IEntityTypeConfiguration<Metadata>
{
    public void Configure(EntityTypeBuilder<Metadata> builder)
    {
        builder.HasKey(column => column.MetadataId);
        builder.Property(column => column.MetadataId)
            .ValueGeneratedOnAdd();
        builder.Property(column => column.GlobalTradeItemNumber)
            .IsRequired();
        builder.HasIndex(column => column.GlobalTradeItemNumber);
        builder.Property(column => column.FoodFacts)
            .HasColumnType("jsonb");
        builder.Property(column => column.ProductFacts)
            .HasColumnType("jsonb");
        builder.Property(column => column.CreatedAt)
            .IsRequired();
        builder.Property(column => column.UpdatedAt);
    }
}
