using Cinema.Api.Contracts;
using Cinema.Api.Mapping;
using Cinema.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/v1/seat-map")]
public sealed class SeatMapController(ISeatMapService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<SeatMapResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status502BadGateway)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await service.GetSeatMapAsync(cancellationToken);
        return result.IsSuccess
            ? Ok(SeatResponseMapper.Map(result.Value))
            : ResultProblemMapper.Map(this, result.Errors);
    }
}
