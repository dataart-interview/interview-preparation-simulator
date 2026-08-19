using System.ComponentModel.DataAnnotations;
using Cinema.Api.Contracts;
using Cinema.Api.Mapping;
using Cinema.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/v1/seats")]
public sealed class SeatsController(ISeatMapService service) : ControllerBase
{
    [HttpGet("{row}/{number:int}")]
    [ProducesResponseType<SeatAvailabilityResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status502BadGateway)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(string row, [Range(1, int.MaxValue)] int number, CancellationToken cancellationToken)
    {
        var result = await service.GetSeatAsync(row, number, cancellationToken);
        if (result.IsFailed)
        {
            return ResultProblemMapper.Map(this, result.Errors);
        }

        return Ok(SeatResponseMapper.Map(result.Value));
    }
}
