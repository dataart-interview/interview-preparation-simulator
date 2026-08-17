using Cinema.Domain.Errors;
using Cinema.Domain.Models;
using Cinema.Infrastructure.Feed.Models;
using FluentResults;

namespace Cinema.Infrastructure.Feed.Mapping;

public static class CinemaFeedMapper
{
    public static Result<SeatMap> Map(IReadOnlyList<SeatMapFeedDto>? source)
    {
        if (source is not [var item] || string.IsNullOrWhiteSpace(item.Auditorium) || string.IsNullOrWhiteSpace(item.FilmTitle) ||
            !long.TryParse(item.StartTime, out var unixSeconds) || item.SeatRows is null || item.SeatRows.Count == 0)
        {
            return Result.Fail<SeatMap>(new InvalidSeatMapError("The upstream seat map does not match the expected contract."));
        }

        var seats = new List<Seat>();
        foreach (var (row, values) in item.SeatRows.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(row)
                || string.IsNullOrEmpty(values)
                || values.Any(value => value is not ('0' or '1')))
            {
                return Result.Fail<SeatMap>(new InvalidSeatMapError("The upstream seat map contains invalid seat rows."));
            }

            for (var index = 0; index < values.Length; index++)
            {
                seats.Add(new Seat(row, index + 1, values[index] == '0' ? SeatStatus.Available : SeatStatus.Booked));
            }
        }

        return new SeatMap(
            item.Auditorium,
            item.FilmTitle,
            DateTimeOffset.FromUnixTimeSeconds(unixSeconds),
            seats);
    }
}
