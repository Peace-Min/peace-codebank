using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using Peace.Codebank.Visualization.Charting;

namespace Peace.Codebank.Tests.Visualization.Charting;

public class ChartColorServiceTests
{
    [Fact]
    public void GenerateUniqueColorReturnsColorWhenExistingItemsAreNull()
    {
        var service = new ChartColorService();

        var color = service.GenerateUniqueColor(null);

        color.IsEmpty.Should().BeFalse();
    }

    [Fact]
    public void GenerateUniqueColorIgnoresEmptyExistingColors()
    {
        var expectedService = new ChartColorService();
        var actualService = new ChartColorService();

        var expected = expectedService.GenerateUniqueColor(Array.Empty<IColoredItem>());
        var actual = actualService.GenerateUniqueColor(new[] { new TestColoredItem(Color.Empty) });

        actual.Should().Be(expected);
    }

    [Fact]
    public void GenerateUniqueColorRetriesWhenFirstCandidateIsAlreadyUsed()
    {
        var seedService = new ChartColorService();
        var service = new ChartColorService();
        var firstCandidate = seedService.GenerateUniqueColor(Array.Empty<IColoredItem>());

        var actual = service.GenerateUniqueColor(new[] { new TestColoredItem(firstCandidate) });

        actual.Should().NotBe(firstCandidate);
        actual.IsEmpty.Should().BeFalse();
    }

    [Fact]
    public void GenerateUniqueColorProducesDistinctColorsForRepeatedAdditions()
    {
        var service = new ChartColorService();
        var items = new List<IColoredItem>();

        for (var i = 0; i < 5; i++)
        {
            var color = service.GenerateUniqueColor(items);
            items.Add(new TestColoredItem(color));
        }

        items.Should().OnlyHaveUniqueItems(item => item.DisplayColor.ToArgb());
    }

    private sealed class TestColoredItem : IColoredItem
    {
        public TestColoredItem(Color displayColor)
        {
            DisplayColor = displayColor;
        }

        public Color DisplayColor { get; }
    }
}
