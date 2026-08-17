using System.ComponentModel.DataAnnotations;
using Cinema.Api.Contracts;
using Cinema.Api.Mapping;
using Cinema.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/v1/adjacent-seats")]
public sealed class AdjacentSeatsController(ISeatMapService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<AdjacentSeatsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status502BadGateway)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get([FromQuery, Range(2, int.MaxValue)] int minSeats = 2, CancellationToken cancellationToken = default)
    {
        var result = await service.FindAdjacentSeatsAsync(minSeats, cancellationToken);
        return result.IsSuccess
            ? Ok(SeatResponseMapper.Map(result.Value))
            : ResultProblemMapper.Map(this, result.Errors);
    }
}
