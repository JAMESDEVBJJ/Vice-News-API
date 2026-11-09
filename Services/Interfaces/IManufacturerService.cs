using ViceNews.API.DTOs;

namespace ViceNews.API.Services.Interfaces;

public interface IManufacturerService
{
    Task<IReadOnlyList<ManufacturerDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
