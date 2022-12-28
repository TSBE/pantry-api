using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence.Configurations;

public class ArticleEntityTypeConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(column => column.ArticleId);
        builder.Property(column => column.ArticleId)
            .ValueGeneratedOnAdd();
        builder.Property(column => column.GlobalTradeItemNumber)
            .IsRequired();
        builder.HasIndex(b => b.GlobalTradeItemNumber);
        builder.Property(column => column.Name)
            .IsRequired();
        builder.HasIndex(column => column.BestBeforeDate);
        builder.Property(column => column.Quantity);
        builder.Property(column => column.Content);
        builder.Property(column => column.ContentType)
            .IsRequired();
        builder.Property(column => column.ImageUrl);
        builder.Ignore(column => column.Metadata);
        builder.Property(column => column.HouseholdId)
            .IsRequired();
        builder.Property(column => column.StorageLocationId)
            .IsRequired();
        builder.Property(column => column.CreatedAt)
            .IsRequired();
        builder.Property(column => column.UpdatedAt);
    }
}
