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
    private static readonly FusionCacheEntryOptions CacheOptions = new FusionCacheEntryOptions(TimeSpan.FromSeconds(5))
        .SetFailSafe(true, TimeSpan.FromSeconds(35));

    public async Task<Result<SeatMap>> GetSeatMapAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await cache.GetOrSetAsync(
                "seat-map",
                async token =>
                {
                    var result = await feedClient.GetSeatMapAsync(token);
                    return result.IsSuccess
                        ? result.Value
                        : throw new FeedException(result.Errors);
                },
                CacheOptions,
                cancellationToken);
        }
        catch (FeedException exception)
        {
            return Result.Fail<SeatMap>(exception.Errors);
        }
    }

    private sealed class FeedException(IReadOnlyList<IError> errors) : Exception
    {
        public IReadOnlyList<IError> Errors { get; } = errors;
    }
}
