namespace Cinema.Domain.Models;

public sealed record SeatMap(
    string Auditorium,
    string FilmTitle,
    DateTimeOffset StartTimeUtc,
    IReadOnlyList<Seat> Seats);
