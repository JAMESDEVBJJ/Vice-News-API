using Microsoft.EntityFrameworkCore;
using ViceNews.API.Data;
using ViceNews.API.DTOs;
using ViceNews.API.Services.Interfaces;

namespace ViceNews.API.Services;

public class VehicleCategoryService : IVehicleCategoryService
{
    private readonly ApplicationDbContext _context;

    public VehicleCategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<VehicleCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VehicleCategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new VehicleCategoryDto(x.Id, x.Name, x.Slug))
            .ToListAsync(cancellationToken);
    }
}
