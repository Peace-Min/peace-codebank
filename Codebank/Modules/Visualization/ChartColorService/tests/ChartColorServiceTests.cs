using System.Collections.Generic;
using System.Windows.Media;
using FluentAssertions;
using Peace.Codebank.Visualization.Charting;

namespace Peace.Codebank.Tests.Visualization.Charting;

public class ChartColorServiceTests
{
    private static readonly Color[] FixedColors =
    {
        Color.FromArgb(255, 242, 24, 24),
        Color.FromArgb(255, 245, 124, 0),
        Color.FromArgb(255, 255, 179, 0),
        Color.FromArgb(255, 255, 235, 59),
        Color.FromArgb(255, 124, 179, 66),
        Color.FromArgb(255, 0, 137, 123),
        Color.FromArgb(255, 3, 155, 229),
        Color.FromArgb(255, 30, 136, 229),
        Color.FromArgb(255, 57, 73, 171),
        Color.FromArgb(255, 142, 36, 170),
    };

    [Fact]
    public void GenerateUniqueColorReturnsFirstRegisteredColorWhenExistingItemsAreNull()
    {
        var service = new ChartColorService();

        var color = service.GenerateUniqueColor(null);

        color.Should().Be(FixedColors[0]);
    }

    [Fact]
    public void GenerateUniqueColorUsesExistingItemCountEvenWhenColorIsEmpty()
    {
        var service = new ChartColorService();

        var actual = service.GenerateUniqueColor(new[] { new TestColoredItem(default(Color)) });

        actual.Should().Be(FixedColors[1]);
    }

    [Fact]
    public void GenerateUniqueColorReturnsFirstRegisteredColorWhenExistingItemsAreEmpty()
    {
        var service = new ChartColorService();

        var color = service.GenerateUniqueColor(Array.Empty<IColoredItem>());

        color.Should().Be(FixedColors[0]);
    }

    [Fact]
    public void GenerateUniqueColorReturnsFixedColorsForFirstTenAdditions()
    {
        var service = new ChartColorService();
        var items = new List<IColoredItem>();

        for (var i = 0; i < FixedColors.Length; i++)
        {
            var actual = service.GenerateUniqueColor(items);
            actual.Should().Be(FixedColors[i]);
            items.Add(new TestColoredItem(actual));
        }
    }

    [Fact]
    public void GenerateUniqueColorFallsBackAfterFixedColorsAreExhausted()
    {
        var service = new ChartColorService();
        var items = new List<IColoredItem>();

        for (var i = 0; i < FixedColors.Length + 1; i++)
        {
            var color = service.GenerateUniqueColor(items);
            items.Add(new TestColoredItem(color));
        }

        items[FixedColors.Length].Color.Should().NotBe(default(Color));
        FixedColors.Should().NotContain(items[FixedColors.Length].Color);
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
