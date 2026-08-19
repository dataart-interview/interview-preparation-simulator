namespace Cinema.Domain.Models;

public sealed record Seat(string Row, int Number, SeatStatus Status);
