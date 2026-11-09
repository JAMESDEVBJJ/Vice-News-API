using Microsoft.AspNetCore.Mvc;
using ViceNews.API.DTOs;
using ViceNews.API.Services.Interfaces;

namespace ViceNews.API.Controllers;

[ApiController]
[Route("api/manufacturers")]
public class ManufacturersController : ControllerBase
{
    private readonly IManufacturerService _service;

    public ManufacturersController(IManufacturerService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ManufacturerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ManufacturerDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }
}
