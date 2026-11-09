using Microsoft.AspNetCore.Mvc;
using ViceNews.API.DTOs;
using ViceNews.API.Services.Interfaces;

namespace ViceNews.API.Controllers;

[ApiController]
[Route("api/vehicle-categories")]
public class VehicleCategoriesController : ControllerBase
{
    private readonly IVehicleCategoryService _service;

    public VehicleCategoriesController(IVehicleCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VehicleCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<VehicleCategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }
}
