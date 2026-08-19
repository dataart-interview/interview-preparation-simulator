namespace Cinema.Api.Contracts;

public sealed record SeatMapResponse(
    string Auditorium,
    string FilmTitle,
    TimeOnly StartTime,
    IReadOnlyList<SeatContract> Seats);
