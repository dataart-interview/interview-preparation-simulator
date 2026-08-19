using Cinema.Domain.Errors;
using Cinema.Domain.Models;
using Cinema.Infrastructure.Feed.Models;
using FluentResults;

namespace Cinema.Infrastructure.Feed.Mapping;

public static class CinemaFeedMapper
{
    public static Result<SeatMap> Map(IReadOnlyList<SeatMapFeedDto>? source)
    {
        if (source is not [var item]
            || !long.TryParse(item.StartTime, out var unixSeconds)
            || item.SeatRows is not { Count: > 0 })
        {
            return Result.Fail<SeatMap>(new InvalidSeatMapError("The upstream seat map does not match the expected contract."));
        }

        if (item.SeatRows.Any(row => !IsValidRow(row.Key, row.Value)))
        {
            return Result.Fail<SeatMap>(new InvalidSeatMapError("The upstream seat map contains invalid seat rows."));
        }

        var seats = item.SeatRows
            .OrderBy(row => row.Key, StringComparer.Ordinal)
            .SelectMany(row => row.Value.Select((value, index) =>
                new Seat(row.Key, index + 1, value == '0' ? SeatStatus.Available : SeatStatus.Booked)))
            .ToList();

        return new SeatMap(
            item.Auditorium,
            item.FilmTitle,
            DateTimeOffset.FromUnixTimeSeconds(unixSeconds),
            seats);
    }

    private static bool IsValidRow(string row, string values) =>
        !string.IsNullOrWhiteSpace(row)
        && !string.IsNullOrEmpty(values)
        && !values.AsSpan().ContainsAnyExcept('0', '1');
}
