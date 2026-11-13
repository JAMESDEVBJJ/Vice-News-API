using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ViceNews.API.Configurations;
public class UtcDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public UtcDateTimeOffsetConverter()
        : base(v => v.ToUniversalTime(), v => v.ToUniversalTime())
    {
    }
}
