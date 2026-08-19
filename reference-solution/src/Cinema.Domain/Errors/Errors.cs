using FluentResults;

namespace Cinema.Domain.Errors;

public sealed class SeatNotFoundError(string row, int number)
    : Error($"Seat {row}{number} was not found.");

public sealed class InvalidSeatMapError(string message)
    : Error(message);

public sealed class UpstreamResponseError()
    : Error("The seat map source returned an unexpected response.");

public sealed class SeatMapUnavailableError()
    : Error("The seat map source is temporarily unavailable.");
