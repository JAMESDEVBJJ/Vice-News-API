using Microsoft.EntityFrameworkCore;
using ViceNews.API.Data;
using ViceNews.API.DTOs;
using ViceNews.API.Services.Interfaces;

namespace ViceNews.API.Services;

public class ManufacturerService : IManufacturerService
{
    private readonly ApplicationDbContext _context;

    public ManufacturerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ManufacturerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Manufacturers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ManufacturerDto(x.Id, x.Name, x.Slug))
            .ToListAsync(cancellationToken);
    }
}
