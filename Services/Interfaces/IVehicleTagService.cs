using ViceNews.API.DTOs;

namespace ViceNews.API.Services.Interfaces;

public interface IVehicleTagService
{
    Task<IReadOnlyList<VehicleTagDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
