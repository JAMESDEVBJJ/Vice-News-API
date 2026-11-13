using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ViceNews.API.Data;
using ViceNews.API.DTOs;
using ViceNews.API.Helpers;
using ViceNews.API.Models;
using ViceNews.API.Services.Interfaces;

namespace ViceNews.API.Services;

public class VehicleService : IVehicleService
{
    // Mirrors VehicleTagConfiguration (Name max length 100).
    private const int TagNameMaxLength = 100;

    private static readonly Expression<Func<Vehicle, VehicleResponseDto>> ToResponse = v => new VehicleResponseDto(
        v.Id,
        v.Name,
        v.Slug,
        v.PriceGta,
        v.Description,
        v.ImageUrl,
        v.MaxSpeed,
        v.Acceleration,
        v.Braking,
        v.Handling,
        v.CreatedAt,
        v.UpdatedAt,
        new ManufacturerDto(v.Manufacturer.Id, v.Manufacturer.Name, v.Manufacturer.Slug),
        new VehicleCategoryDto(v.Category.Id, v.Category.Name, v.Category.Slug),
        v.VehicleTagAssignments
            .OrderBy(a => a.VehicleTag.Name)
            .Select(a => new VehicleTagDto(a.VehicleTag.Id, a.VehicleTag.Name, a.VehicleTag.Slug))
            .ToList());

    private readonly ApplicationDbContext _context;

    public VehicleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<VehicleResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .OrderByDescending(v => v.CreatedAt)
            .ThenByDescending(v => v.Id)
            .Select(ToResponse)
            .ToListAsync(cancellationToken);
    }

    public async Task<VehicleResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Where(v => v.Id == id)
            .Select(ToResponse)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ServiceResult<VehicleResponseDto>> CreateAsync(
        CreateVehicleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var prepared = await PrepareAsync(request, cancellationToken);
        if (!prepared.Succeeded)
        {
            return prepared.Error;
        }

        var data = prepared.Value;

        // Tag creation runs as its own statement, so wrap it with the vehicle insert.
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var vehicle = new Vehicle
        {
            Slug = await GenerateUniqueVehicleSlugAsync(data.Name, excludeVehicleId: 0, cancellationToken),
            CreatedAt = now,
            UpdatedAt = now,
        };
        ApplyEditableFields(vehicle, data);

        var tagIds = await ResolveTagIdsAsync(data.Tags, cancellationToken);
        SyncTagAssignments(vehicle, tagIds);

        _context.Vehicles.Add(vehicle);

        if (!await TrySaveChangesAsync(cancellationToken))
        {
            return SlugConflict();
        }

        await transaction.CommitAsync(cancellationToken);

        return ServiceResult<VehicleResponseDto>.Success((await GetByIdAsync(vehicle.Id, cancellationToken))!);
    }

    public async Task<ServiceResult<VehicleResponseDto>> UpdateAsync(
        int id,
        UpdateVehicleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.VehicleTagAssignments)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (vehicle is null)
        {
            return ServiceError.NotFound("Veículo não encontrado.");
        }

        var prepared = await PrepareAsync(request, cancellationToken);
        if (!prepared.Succeeded)
        {
            return prepared.Error;
        }

        var data = prepared.Value;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        if (!string.Equals(vehicle.Name, data.Name, StringComparison.Ordinal))
        {
            vehicle.Slug = await GenerateUniqueVehicleSlugAsync(data.Name, vehicle.Id, cancellationToken);
        }

        ApplyEditableFields(vehicle, data);
        vehicle.UpdatedAt = DateTimeOffset.UtcNow;

        var tagIds = await ResolveTagIdsAsync(data.Tags, cancellationToken);
        SyncTagAssignments(vehicle, tagIds);

        if (!await TrySaveChangesAsync(cancellationToken))
        {
            return SlugConflict();
        }

        await transaction.CommitAsync(cancellationToken);

        return ServiceResult<VehicleResponseDto>.Success((await GetByIdAsync(vehicle.Id, cancellationToken))!);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // VehicleTagAssignments are removed by the ON DELETE CASCADE foreign key.
        // Manufacturer, VehicleCategory and VehicleTag rows are never touched.
        var deleted = await _context.Vehicles
            .Where(v => v.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted > 0;
    }

    /// <summary>
    /// Validates and normalizes the request, and checks that the referenced manufacturer and category exist.
    /// </summary>
    private async Task<ServiceResult<VehicleData>> PrepareAsync(
        VehicleRequestDtoBase request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (SlugGenerator.Generate(name).Length == 0)
        {
            return ServiceError.Validation(nameof(request.Name), "O nome deve conter letras ou números.");
        }

        var imageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
        if (imageUrl is not null && !IsHttpUrl(imageUrl))
        {
            return ServiceError.Validation(nameof(request.ImageUrl), "Informe uma URL de imagem HTTP ou HTTPS válida.");
        }

        var tags = NormalizeTags(request.Tags);
        if (!tags.Succeeded)
        {
            return tags.Error;
        }

        var manufacturerId = request.ManufacturerId!.Value;
        if (!await _context.Manufacturers.AnyAsync(m => m.Id == manufacturerId, cancellationToken))
        {
            return ServiceError.NotFound($"Fabricante {manufacturerId} não encontrado.");
        }

        var categoryId = request.CategoryId!.Value;
        if (!await _context.VehicleCategories.AnyAsync(c => c.Id == categoryId, cancellationToken))
        {
            return ServiceError.NotFound($"Categoria {categoryId} não encontrada.");
        }

        return ServiceResult<VehicleData>.Success(new VehicleData(
            name,
            manufacturerId,
            categoryId,
            request.PriceGta!.Value,
            request.Description.Trim(),
            request.MaxSpeed!.Value,
            request.Acceleration!.Value,
            request.Braking!.Value,
            request.Handling!.Value,
            imageUrl,
            tags.Value));
    }

    private static void ApplyEditableFields(Vehicle vehicle, VehicleData data)
    {
        vehicle.Name = data.Name;
        vehicle.ManufacturerId = data.ManufacturerId;
        vehicle.CategoryId = data.CategoryId;
        vehicle.PriceGta = data.PriceGta;
        vehicle.Description = data.Description;
        vehicle.MaxSpeed = data.MaxSpeed;
        vehicle.Acceleration = data.Acceleration;
        vehicle.Braking = data.Braking;
        vehicle.Handling = data.Handling;
        vehicle.ImageUrl = data.ImageUrl;
    }

    /// <summary>
    /// Trims and collapses whitespace, drops empty entries and removes duplicates by slug,
    /// so "SUPER", " super " and "Super" collapse into one tag (first spelling wins).
    /// </summary>
    private static ServiceResult<IReadOnlyList<NormalizedTag>> NormalizeTags(IReadOnlyList<string?>? rawTags)
    {
        var tags = new List<NormalizedTag>();
        var seenSlugs = new HashSet<string>(StringComparer.Ordinal);

        foreach (var raw in rawTags ?? [])
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                continue;
            }

            var name = string.Join(' ', raw.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            if (name.Length > TagNameMaxLength)
            {
                return ServiceError.Validation("Tags", $"A tag \"{name}\" deve ter no máximo {TagNameMaxLength} caracteres.");
            }

            var slug = SlugGenerator.Generate(name);
            if (slug.Length == 0)
            {
                return ServiceError.Validation("Tags", $"A tag \"{name}\" deve conter letras ou números.");
            }

            if (seenSlugs.Add(slug))
            {
                tags.Add(new NormalizedTag(name, slug));
            }
        }

        return ServiceResult<IReadOnlyList<NormalizedTag>>.Success(tags);
    }

    /// <summary>
    /// Returns the ids of the given tags, creating the missing ones. Must run inside the caller's transaction.
    /// </summary>
    private async Task<IReadOnlyList<int>> ResolveTagIdsAsync(
        IReadOnlyList<NormalizedTag> tags,
        CancellationToken cancellationToken)
    {
        if (tags.Count == 0)
        {
            return [];
        }

        var slugs = tags.Select(t => t.Slug).ToArray();

        var existingSlugs = await _context.VehicleTags
            .AsNoTracking()
            .Where(t => slugs.Contains(t.Slug))
            .Select(t => t.Slug)
            .ToListAsync(cancellationToken);

        var missing = tags.Where(t => !existingSlugs.Contains(t.Slug)).ToArray();

        if (missing.Length > 0)
        {
            var names = missing.Select(t => t.Name).ToArray();
            var missingSlugs = missing.Select(t => t.Slug).ToArray();

            // ON CONFLICT relies on the unique Slug index, so a tag created concurrently by another
            // request is reused instead of failing this one. Table/columns follow VehicleTagConfiguration.
            await _context.Database.ExecuteSqlAsync(
                $"""
                INSERT INTO "VehicleTags" ("Name", "Slug")
                SELECT * FROM unnest({names}, {missingSlugs})
                ON CONFLICT ("Slug") DO NOTHING
                """,
                cancellationToken);
        }

        return await _context.VehicleTags
            .AsNoTracking()
            .Where(t => slugs.Contains(t.Slug))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds missing assignments and removes the ones no longer requested.
    /// VehicleTag catalog rows are kept even when they lose their last vehicle.
    /// </summary>
    private static void SyncTagAssignments(Vehicle vehicle, IReadOnlyList<int> tagIds)
    {
        var desired = tagIds.ToHashSet();

        foreach (var assignment in vehicle.VehicleTagAssignments.Where(a => !desired.Contains(a.VehicleTagId)).ToList())
        {
            vehicle.VehicleTagAssignments.Remove(assignment);
        }

        var current = vehicle.VehicleTagAssignments.Select(a => a.VehicleTagId).ToHashSet();

        foreach (var tagId in desired.Where(tagId => !current.Contains(tagId)))
        {
            vehicle.VehicleTagAssignments.Add(new VehicleTagAssignment { VehicleTagId = tagId });
        }
    }

    /// <summary>
    /// Builds "base", "base-2", "base-3"... skipping slugs already used by other vehicles.
    /// </summary>
    private async Task<string> GenerateUniqueVehicleSlugAsync(
        string name,
        int excludeVehicleId,
        CancellationToken cancellationToken)
    {
        var baseSlug = SlugGenerator.Generate(name);
        var prefix = baseSlug + "-";

        var takenSlugs = (await _context.Vehicles
                .AsNoTracking()
                .Where(v => v.Id != excludeVehicleId && (v.Slug == baseSlug || v.Slug.StartsWith(prefix)))
                .Select(v => v.Slug)
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.Ordinal);

        if (!takenSlugs.Contains(baseSlug))
        {
            return baseSlug;
        }

        for (var suffix = 2; ; suffix++)
        {
            var candidate = $"{baseSlug}-{suffix}";
            if (!takenSlugs.Contains(candidate))
            {
                return candidate;
            }
        }
    }

    /// <summary>
    /// Saves pending changes; returns false when a concurrent request took the same vehicle slug.
    /// </summary>
    private async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return false;
        }
    }

    private static ServiceError SlugConflict() =>
        ServiceError.Conflict("Outro veículo com o mesmo nome foi salvo ao mesmo tempo. Tente novamente.");

    private static bool IsHttpUrl(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    private sealed record NormalizedTag(string Name, string Slug);

    private sealed record VehicleData(
        string Name,
        int ManufacturerId,
        int CategoryId,
        long PriceGta,
        string Description,
        short MaxSpeed,
        short Acceleration,
        short Braking,
        short Handling,
        string? ImageUrl,
        IReadOnlyList<NormalizedTag> Tags);
}
