using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fenicia.Common.Data;

public static class PostgresDateTimeOffsetSupport
{
    public static void Init(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        foreach (var property in from entityType in modelBuilder.Model.GetEntityTypes()
                                 from property in entityType.GetProperties().Where(p => p.ClrType == typeof(DateTime))
                                 select property)
        {
            property.SetValueConverter(dateTimeConverter);
        }
    }
}
