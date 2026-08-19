using Cinema.Domain.Models;
using FluentResults;

namespace Cinema.Domain.Services;

public interface ISeatMapProvider
{
    Task<Result<SeatMap>> GetSeatMapAsync(CancellationToken cancellationToken);
}
