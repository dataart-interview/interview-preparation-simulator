using Cinema.Domain.Errors;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Mapping;

public static class ResultProblemMapper
{
    public static IActionResult Map(ControllerBase controller, IReadOnlyList<IError> errors)
    {
        var status = errors switch
        {
            [SeatNotFoundError, ..] => StatusCodes.Status404NotFound,
            [SeatMapUnavailableError, ..] => StatusCodes.Status503ServiceUnavailable,
            [InvalidSeatMapError or UpstreamResponseError, ..] => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError,
        };

        return controller.Problem(statusCode: status);
    }
}
