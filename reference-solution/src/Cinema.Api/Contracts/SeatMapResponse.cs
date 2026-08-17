namespace Cinema.Api.Contracts;

public sealed record SeatMapResponse(
    string Auditorium,
    string FilmTitle,
    string StartTime,
    IReadOnlyList<SeatContract> Seats);
