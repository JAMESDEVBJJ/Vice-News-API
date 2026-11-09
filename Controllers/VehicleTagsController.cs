using Microsoft.AspNetCore.Mvc;
using ViceNews.API.DTOs;
using ViceNews.API.Services.Interfaces;

namespace ViceNews.API.Controllers;

[ApiController]
[Route("api/vehicle-tags")]
public class VehicleTagsController : ControllerBase
{
    private readonly IVehicleTagService _service;

    public VehicleTagsController(IVehicleTagService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VehicleTagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<VehicleTagDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }
}
