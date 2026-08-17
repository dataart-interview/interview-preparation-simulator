using Cinema.Domain.Errors;
using Cinema.Domain.Models;
using FluentResults;

namespace Cinema.Domain.Services;

public sealed class SeatMapService(ISeatMapProvider provider) : ISeatMapService
{
    public Task<Result<SeatMap>> GetSeatMapAsync(CancellationToken cancellationToken) =>
        provider.GetSeatMapAsync(cancellationToken);

    public async Task<Result<Seat>> GetSeatAsync(
        string row,
        int number,
        CancellationToken cancellationToken)
    {
        var mapResult = await provider.GetSeatMapAsync(cancellationToken);
        if (mapResult.IsFailed)
        {
            return Result.Fail<Seat>(mapResult.Errors);
        }

        var seat = mapResult.Value.Seats.FirstOrDefault(candidate =>
            string.Equals(candidate.Row, row, StringComparison.OrdinalIgnoreCase)
            && candidate.Number == number);

        if (seat is null)
        {
            return Result.Fail<Seat>(new SeatNotFoundError(row, number));
        }

        return seat;
    }

    public async Task<Result<AdjacentSeatBlock?>> FindAdjacentSeatsAsync(
        int minSeats,
        CancellationToken cancellationToken)
    {
        var mapResult = await provider.GetSeatMapAsync(cancellationToken);
        if (mapResult.IsFailed)
        {
            return Result.Fail<AdjacentSeatBlock?>(mapResult.Errors);
        }

        return FindBlock(mapResult.Value.Seats, minSeats);
    }

    private static AdjacentSeatBlock? FindBlock(IReadOnlyList<Seat> seats, int minSeats)
    {
        foreach (var row in seats.GroupBy(seat => seat.Row))
        {
            var availableCount = 0;

            foreach (var seat in row)
            {
                availableCount = seat.Status == SeatStatus.Available
                    ? availableCount + 1
                    : 0;

                if (availableCount == minSeats)
                {
                    return new AdjacentSeatBlock(
                        row.Key,
                        seat.Number - minSeats + 1,
                        seat.Number);
                }
            }
        }

        return null;
    }
}
