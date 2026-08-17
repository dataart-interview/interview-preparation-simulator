using Cinema.Infrastructure.Feed.Mapping;
using Cinema.Infrastructure.Feed.Models;

namespace Cinema.UnitTests.Infrastructure;

public sealed class CinemaFeedMapperTests
{
    [Fact]
    public void MapExpandsValidFeedInOrdinalOrder()
    {
        var source = new[] { new SeatMapFeedDto("Main", "Film", "1753804800", new Dictionary<string, string> { ["B"] = "01", ["A"] = "10" }) };
        var result = CinemaFeedMapper.Map(source);

        Assert.True(result.IsSuccess);
        Assert.Collection(result.Value.Seats,
            seat => Assert.Equal(("A", 1, Cinema.Domain.Models.SeatStatus.Booked), (seat.Row, seat.Number, seat.Status)),
            seat => Assert.Equal(("A", 2, Cinema.Domain.Models.SeatStatus.Available), (seat.Row, seat.Number, seat.Status)),
            seat => Assert.Equal(("B", 1, Cinema.Domain.Models.SeatStatus.Available), (seat.Row, seat.Number, seat.Status)),
            seat => Assert.Equal(("B", 2, Cinema.Domain.Models.SeatStatus.Booked), (seat.Row, seat.Number, seat.Status)));
    }

    [Fact]
    public void MapPreservesDescriptiveRowLabel()
    {
        var source = new[]
        {
            new SeatMapFeedDto(
                "Main",
                "Film",
                "1753804800",
                new Dictionary<string, string> { ["Balcony"] = "01" }),
        };

        var result = CinemaFeedMapper.Map(source);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value.Seats, seat => Assert.Equal("Balcony", seat.Row));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1021")]
    [InlineData("10x1")]
    public void MapReturnsErrorForInvalidSeatRow(string rowValue)
    {
        var source = new[] { new SeatMapFeedDto("Main", "Film", "1753804800", new Dictionary<string, string> { ["A"] = rowValue }) };
        var result = CinemaFeedMapper.Map(source);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, error => error is Cinema.Domain.Errors.InvalidSeatMapError);
    }
}
