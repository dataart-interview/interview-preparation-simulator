using Cinema.Domain.Models;
using Cinema.Domain.Errors;
using Cinema.Domain.Services;
using FluentResults;
using NSubstitute;

namespace Cinema.UnitTests.Domain;

public sealed class SeatMapServiceTests
{
    [Fact]
    public async Task GetSeatAsync_LowercaseExistingRow_ReturnsNormalizedSeat()
    {
        var provider = ProviderWith(new Seat("B", 3, SeatStatus.Available));
        var result = await new SeatMapService(provider).GetSeatAsync("b", 3, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("B", result.Value.Row);
        Assert.Equal(3, result.Value.Number);
    }

    [Fact]
    public async Task GetSeatAsync_MultiCharacterRow_MatchesIgnoringCase()
    {
        var provider = ProviderWith(new Seat("Balcony", 3, SeatStatus.Available));

        var result = await new SeatMapService(provider)
            .GetSeatAsync("balcony", 3, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Balcony", result.Value.Row);
    }

    [Fact]
    public async Task GetSeatAsync_MissingSeat_ReturnsSeatNotFoundError()
    {
        var provider = ProviderWith(new Seat("A", 1, SeatStatus.Booked));
        var result = await new SeatMapService(provider).GetSeatAsync("A", 2, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is SeatNotFoundError);
    }

    [Fact]
    public async Task FindAdjacentSeatsAsync_AvailableRun_ReturnsFirstExactSizeBlock()
    {
        var provider = ProviderWith(
            new Seat("A", 1, SeatStatus.Available), new Seat("A", 2, SeatStatus.Available),
            new Seat("A", 3, SeatStatus.Available), new Seat("B", 1, SeatStatus.Available));
        var result = await new SeatMapService(provider).FindAdjacentSeatsAsync(2, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new AdjacentSeatBlock("A", 1, 2), result.Value);
    }

    [Fact]
    public async Task FindAdjacentSeatsAsync_DoesNotCrossRows()
    {
        var provider = ProviderWith(new Seat("A", 1, SeatStatus.Available), new Seat("B", 1, SeatStatus.Available));
        var result = await new SeatMapService(provider).FindAdjacentSeatsAsync(2, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    private static ISeatMapProvider ProviderWith(params Seat[] seats)
    {
        var provider = Substitute.For<ISeatMapProvider>();
        var map = new SeatMap("Main", "Film", DateTimeOffset.UnixEpoch, seats);
        provider.GetSeatMapAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Ok(map));
        return provider;
    }
}
