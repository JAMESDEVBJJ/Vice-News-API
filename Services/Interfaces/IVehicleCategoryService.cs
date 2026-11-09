using ViceNews.API.DTOs;

namespace ViceNews.API.Services.Interfaces;

public interface IVehicleCategoryService
{
    Task<IReadOnlyList<VehicleCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
