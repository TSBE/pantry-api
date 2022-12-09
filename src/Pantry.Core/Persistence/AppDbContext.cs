using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pantry.Common.EntityFrameworkCore.Converters;
using Pantry.Common.Time;
using Pantry.Core.Models.EanSearchOrg;
using Pantry.Core.Models.OpenFoodFacts;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; } = null!;

    public DbSet<Article> Articles { get; set; } = null!;

    public DbSet<Device> Devices { get; set; } = null!;

    public DbSet<Household> Households { get; set; } = null!;

    public DbSet<Invitation> Invitations { get; set; } = null!;

    public DbSet<StorageLocation> StorageLocations { get; set; } = null!;

    public DbSet<Metadata> Metadatas { get; set; } = null!;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditableEntityProperties();

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyUtcDateTimeConverterToAllDateTimeProperties();

        if (Database.IsSqlite())
        {
            // Use for unit testing only https://github.com/dotnet/efcore/issues/28816
            modelBuilder.Entity<Metadata>().Property(x => x.FoodFacts).HasColumnType("text")
                .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Product>(v, (JsonSerializerOptions)null!));
            modelBuilder.Entity<Metadata>().Property(x => x.ProductFacts).HasColumnType("text")
                .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<NonFoodProduct>(v, (JsonSerializerOptions)null!));
        }
    }

    private void SetAuditableEntityProperties()
    {
        // get entries that are being Added or Updated
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        var now = DateTimeProvider.UtcNow;

        foreach (var entry in modifiedEntries)
        {
            if (entry.Entity is not Auditable entity)
            {
                continue;
            }

            entity.UpdateDateTime(entry.State, now);
        }
    }
}
