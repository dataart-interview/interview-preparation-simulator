using Cinema.Domain.Models;
using FluentResults;

namespace Cinema.Infrastructure.Feed;

public interface ICinemaFeedClient
{
    Task<Result<SeatMap>> GetSeatMapAsync(CancellationToken cancellationToken);
}
