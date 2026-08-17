namespace Cinema.Infrastructure.Feed.Models;

public sealed record SeatMapFeedDto(
    string Auditorium,
    string FilmTitle,
    string? StartTime,
    Dictionary<string, string>? SeatRows);
