namespace Cinema.Api.Contracts;

public sealed record AdjacentSeatsResponse(
    bool Found,
    string? Row,
    int? StartNumber,
    int? EndNumber);
