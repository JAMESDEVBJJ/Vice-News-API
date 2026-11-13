namespace ViceNews.API.DTOs;

public record VehicleResponseDto(
    int Id,
    string Name,
    string Slug,
    long PriceGta,
    string Description,
    string? ImageUrl,
    short MaxSpeed,
    short Acceleration,
    short Braking,
    short Handling,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    ManufacturerDto Manufacturer,
    VehicleCategoryDto Category,
    IReadOnlyList<VehicleTagDto> Tags);
