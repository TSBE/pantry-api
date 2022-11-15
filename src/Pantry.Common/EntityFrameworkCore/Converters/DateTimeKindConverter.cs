using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pantry.Common.EntityFrameworkCore.Converters;

/// <summary>
///     Converts DateTime Values from the Database to <see cref="DateTimeKind.Utc" />. It does not modify or convert the DateTime
///     itself, it just set the kind (and assumes that Dates in the Database are in UTC).
/// </summary>
public static class DateTimeKindConverter
{
    /// <summary>
    ///     Converts the Kind of DateTime Values loaded from the Database to Utc without changing the actual DateTime value.
    /// </summary>
    public static readonly ValueConverter<DateTime, DateTime> UtcKindConverter = new(v => EnsureUtc(v), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    /// <summary>
    ///     Applies the <see cref="UtcKindConverter" /> to all configured DateTime properties of the given ModelBuilder.
    /// </summary>
    public static void ApplyUtcDateTimeConverterToAllDateTimeProperties(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(UtcKindConverter);
                }
            }
        }
    }

    private static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
        {
            throw new InvalidDateTimeKindException($"The given DateTime Value is not UTC (actual Kind {dateTime.Kind:G}).");
        }

        return dateTime;
    }
}
