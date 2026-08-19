using Cinema.Api.Contracts;
using Cinema.Domain.Models;

namespace Cinema.Api.Mapping;

public static class SeatResponseMapper
{
    public static SeatMapResponse Map(SeatMap seatMap) => new(
        seatMap.Auditorium,
        seatMap.FilmTitle,
        TimeOnly.FromDateTime(seatMap.StartTimeUtc.UtcDateTime),
        [.. seatMap.Seats.Select(MapSeat)]);

    public static SeatAvailabilityResponse Map(Seat seat) => new(
        seat.Status == SeatStatus.Available);

    public static AdjacentSeatsResponse Map(AdjacentSeatBlock? block) => new(
        block is not null,
        block?.Row,
        block?.StartNumber,
        block?.EndNumber);

    private static SeatContract MapSeat(Seat seat) => new(
        seat.Row,
        seat.Number,
        seat.Status.ToString().ToLowerInvariant());
}
