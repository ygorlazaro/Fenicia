using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fenicia.Common.Data;

public static class PostgresDateTimeOffsetSupport
{
    public static void Init(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

        foreach (var property in from et in modelBuilder.Model.GetEntityTypes()
                                  from p in et.GetProperties().Where(p => p.ClrType == typeof(DateTime))
                                  select p)
        {
            property.SetValueConverter(dateTimeConverter);
        }

        foreach (var property in from et in modelBuilder.Model.GetEntityTypes()
                                  from p in et.GetProperties().Where(p => p.ClrType == typeof(DateTime?))
                                  select p)
        {
            property.SetValueConverter(nullableDateTimeConverter);
        }
    }
}