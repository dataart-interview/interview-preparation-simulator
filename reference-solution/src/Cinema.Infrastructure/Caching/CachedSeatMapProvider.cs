using Cinema.Domain.Models;
using Cinema.Domain.Services;
using Cinema.Infrastructure.Feed;
using FluentResults;
using ZiggyCreatures.Caching.Fusion;

namespace Cinema.Infrastructure.Caching;

public sealed class CachedSeatMapProvider(
    ICinemaFeedClient feedClient,
    IFusionCache cache) : ISeatMapProvider
{
    private const string CacheKey = "seat-map";

    public async Task<Result<SeatMap>> GetSeatMapAsync(CancellationToken cancellationToken) =>
        await cache.GetOrSetAsync(
            CacheKey,
            feedClient.GetSeatMapAsync,
            TimeSpan.FromSeconds(5),
            cancellationToken);
}
