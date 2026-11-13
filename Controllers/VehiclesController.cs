using Microsoft.AspNetCore.Mvc;
using ViceNews.API.DTOs;
using ViceNews.API.Services;
using ViceNews.API.Services.Interfaces;

namespace ViceNews.API.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _service;

    public VehiclesController(IVehicleService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VehicleResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<VehicleResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var vehicle = await _service.GetByIdAsync(id, cancellationToken);
        return vehicle is null ? NotFound() : Ok(vehicle);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VehicleResponseDto>> Create(
        CreateVehicleRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return ToErrorResult(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VehicleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VehicleResponseDto>> Update(
        int id,
        UpdateVehicleRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : ToErrorResult(result.Error);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await _service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }

    // Translates expected service failures into ProblemDetails responses.
    private ActionResult ToErrorResult(ServiceError error)
    {
        switch (error.Type)
        {
            case ServiceErrorType.Validation:
                ModelState.AddModelError(error.Field ?? string.Empty, error.Message);
                return ValidationProblem(ModelState);
            case ServiceErrorType.NotFound:
                return Problem(detail: error.Message, statusCode: StatusCodes.Status404NotFound);
            case ServiceErrorType.Conflict:
                return Problem(detail: error.Message, statusCode: StatusCodes.Status409Conflict);
            default:
                throw new ArgumentOutOfRangeException(nameof(error), error.Type, "Unknown service error type.");
        }
    }
}
