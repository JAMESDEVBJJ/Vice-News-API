using Microsoft.EntityFrameworkCore;
using ViceNews.API.Data;
using ViceNews.API.DTOs;
using ViceNews.API.Services.Interfaces;

namespace ViceNews.API.Services;

public class VehicleTagService : IVehicleTagService
{
    private readonly ApplicationDbContext _context;

    public VehicleTagService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<VehicleTagDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VehicleTags
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new VehicleTagDto(x.Id, x.Name, x.Slug))
            .ToListAsync(cancellationToken);
    }
}
