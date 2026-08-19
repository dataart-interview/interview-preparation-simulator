using System.Net;
using System.Text.Json;
using Cinema.Domain.Errors;
using Cinema.Domain.Models;
using Cinema.Infrastructure.Feed.Mapping;
using Cinema.Infrastructure.Feed.Models;
using FluentResults;
using Microsoft.Extensions.Logging;
using Polly;

namespace Cinema.Infrastructure.Feed;

public sealed partial class CinemaFeedClient(
    HttpClient httpClient,
    ILogger<CinemaFeedClient> logger) : ICinemaFeedClient
{
    private const string SeatMapPath = "/dataart-interview/interview-technical-exercise-dotnet/main/seatmap-example.json";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true,
    };

    public async Task<Result<SeatMap>> GetSeatMapAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(
                SeatMapPath,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                LogFinalStatus(logger, (int)response.StatusCode);
                return IsTransient(response.StatusCode)
                    ? Result.Fail<SeatMap>(new SeatMapUnavailableError())
                    : Result.Fail<SeatMap>(new UpstreamResponseError());
            }

            await using var body = await response.Content.ReadAsStreamAsync(cancellationToken);
            var feed = await JsonSerializer.DeserializeAsync<List<SeatMapFeedDto>>(body, JsonOptions, cancellationToken);
            return CinemaFeedMapper.Map(feed);
        }
        catch (JsonException)
        {
            return Result.Fail<SeatMap>(new InvalidSeatMapError("The upstream seat map contains invalid JSON."));
        }
        catch (Exception exception) when (
            exception is HttpRequestException or ExecutionRejectedException
            || exception is OperationCanceledException && !cancellationToken.IsCancellationRequested)
        {
            return Result.Fail<SeatMap>(new SeatMapUnavailableError());
        }
    }

    private static bool IsTransient(HttpStatusCode statusCode) =>
        statusCode == HttpStatusCode.NotFound
        || statusCode == HttpStatusCode.RequestTimeout
        || statusCode == (HttpStatusCode)429
        || (int)statusCode >= 500;

    [LoggerMessage(LogLevel.Warning, "Cinema feed returned HTTP {StatusCode}")]
    private static partial void LogFinalStatus(ILogger logger, int statusCode);
}
