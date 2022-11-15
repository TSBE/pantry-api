using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pantry.Common.EntityFrameworkCore.Converters;
using Pantry.Common.Time;
using Pantry.Core.Persistence.Entities;

namespace Pantry.Core.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; } = null!;

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
