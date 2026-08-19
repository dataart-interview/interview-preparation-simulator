using Cinema.Domain.Models;
using FluentResults;

namespace Cinema.Domain.Services;

public interface ISeatMapService
{
    Task<Result<SeatMap>> GetSeatMapAsync(CancellationToken cancellationToken);

    Task<Result<Seat>> GetSeatAsync(string row, int number, CancellationToken cancellationToken);

    Task<Result<AdjacentSeatBlock?>> FindAdjacentSeatsAsync(int minSeats, CancellationToken cancellationToken);
}
