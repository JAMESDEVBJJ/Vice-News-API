using ViceNews.API.DTOs;

namespace ViceNews.API.Services.Interfaces;

public interface IVehicleService
{
    Task<IReadOnlyList<VehicleResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<VehicleResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<VehicleResponseDto>> CreateAsync(CreateVehicleRequestDto request, CancellationToken cancellationToken = default);
    Task<ServiceResult<VehicleResponseDto>> UpdateAsync(int id, UpdateVehicleRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
