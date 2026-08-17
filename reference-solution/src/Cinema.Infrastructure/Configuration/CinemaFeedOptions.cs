using System.ComponentModel.DataAnnotations;

namespace Cinema.Infrastructure.Configuration;

public sealed class CinemaFeedOptions
{
    public const string SectionName = "CinemaFeed";

    [Required]
    public required Uri BaseAddress { get; init; }
}
