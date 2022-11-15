using System;
using Microsoft.EntityFrameworkCore;
using Pantry.Common.Time;

namespace Pantry.Core.Persistence.Entities;

public abstract class Auditable
{
    public DateTime CreatedAt { get; private set; } = DateTimeProvider.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    public void UpdateDateTime(EntityState state, DateTime dateTime)
    {
        switch (state)
        {
            case EntityState.Added:
                CreatedAt = dateTime;
                break;
            case EntityState.Modified:
                UpdatedAt = dateTime;
                break;
        }
    }
}
