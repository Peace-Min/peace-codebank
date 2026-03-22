using System.Collections.Generic;
using System.Windows.Media;
using FluentAssertions;
using Peace.Codebank.Visualization.Charting;

namespace Peace.Codebank.Tests.Visualization.Charting;

public class ChartColorServiceTests
{
    [Fact]
    public void GenerateUniqueColorReturnsFirstRegisteredColorWhenExistingItemsAreNull()
    {
        var service = new ChartColorService();

        var color = service.GenerateUniqueColor(null);

        color.Should().Be(Color.FromArgb(255, 242, 24, 24));
    }

    [Fact]
    public void GenerateUniqueColorIgnoresEmptyExistingColors()
    {
        var expectedService = new ChartColorService();
        var actualService = new ChartColorService();

        var expected = expectedService.GenerateUniqueColor(Array.Empty<IColoredItem>());
        var actual = actualService.GenerateUniqueColor(new[] { new TestColoredItem(default(Color)) });

        actual.Should().Be(expected);
    }

    [Fact]
    public void GenerateUniqueColorReturnsFirstRegisteredColorWhenExistingItemsAreEmpty()
    {
        var service = new ChartColorService();

        var color = service.GenerateUniqueColor(Array.Empty<IColoredItem>());

        color.Should().Be(Color.FromArgb(255, 242, 24, 24));
    }

    [Fact]
    public void GenerateUniqueColorRetriesWhenFirstCandidateIsAlreadyUsed()
    {
        var seedService = new ChartColorService();
        var service = new ChartColorService();
        var firstCandidate = seedService.GenerateUniqueColor(Array.Empty<IColoredItem>());

        var actual = service.GenerateUniqueColor(new[] { new TestColoredItem(firstCandidate) });

        actual.Should().NotBe(firstCandidate);
        actual.Should().NotBe(default(Color));
    }

    [Fact]
    public void GenerateUniqueColorProducesDistinctColorsForFirstElevenAdditions()
    {
        var service = new ChartColorService();
        var items = new List<IColoredItem>();

        for (var i = 0; i < 11; i++)
        {
            var color = service.GenerateUniqueColor(items);
            items.Add(new TestColoredItem(color));
        }

        items.Should().OnlyHaveUniqueItems(item => ToArgb(item.Color));
    }

    [Fact]
    public void GenerateUniqueColorKeepsUsingPreferredColorsWhenAUserDefinedColorExists()
    {
        var service = new ChartColorService();
        var items = new List<IColoredItem>
        {
            new TestColoredItem(service.GenerateUniqueColor(null)),
            new TestColoredItem(Color.FromArgb(255, 34, 34, 34)),
        };

        while (items.Count < 11)
        {
            var color = service.GenerateUniqueColor(items);
            items.Add(new TestColoredItem(color));
        }

        items.Should().OnlyHaveUniqueItems(item => ToArgb(item.Color));
    }

    [Fact]
    public void GenerateUniqueColorFallsBackAfterPreferredColorsAreExhausted()
    {
        var service = new ChartColorService();
        var items = new List<IColoredItem>();

        for (var i = 0; i < 12; i++)
        {
            var color = service.GenerateUniqueColor(items);
            items.Add(new TestColoredItem(color));
        }

        items.Should().OnlyHaveUniqueItems(item => ToArgb(item.Color));
        items[11].Color.Should().NotBe(default(Color));
    }

    private sealed class TestColoredItem : IColoredItem
    {
        public TestColoredItem(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }

    private static int ToArgb(Color color)
    {
        return
            (color.A << 24) |
            (color.R << 16) |
            (color.G << 8) |
            color.B;
    }
}
