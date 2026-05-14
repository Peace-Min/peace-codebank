using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Peace.Codebank.Visualization.Charting;

public sealed class ChartColorService
{
    private const double GoldenRatioConjugate = 0.618033988749895;
    private const int ColorSimilarityThreshold = 75;
    private const int MaxRetryAttempts = 10;
    private const double DefaultSaturation = 0.8;
    private const double DefaultBrightness = 0.6;
    private static readonly Color[] FixedColors =
    {
        Color.FromArgb(255, 242, 24, 24), // 0: 빨강.
        Color.FromArgb(255, 245, 124, 0), // 1: 주황.
        Color.FromArgb(255, 255, 179, 0), // 2: 노랑.
        Color.FromArgb(255, 255, 235, 59), // 3: 연노랑.
        Color.FromArgb(255, 124, 179, 66), // 4: 연두.
        Color.FromArgb(255, 0, 137, 123), // 5: 청록.
        Color.FromArgb(255, 3, 155, 229), // 6: 하늘.
        Color.FromArgb(255, 30, 136, 229), // 7: 파랑.
        Color.FromArgb(255, 57, 73, 171), // 8: 남색.
        Color.FromArgb(255, 142, 36, 170), // 9: 보라.
    };

    private double _hueStep;

    public Color GenerateUniqueColor(IEnumerable<IColoredItem>? existingItems)
    {
        var existingItemCount = GetExistingItemCount(existingItems);
        var usedColors = GetUsedColors(existingItems);

        if (existingItemCount < FixedColors.Length)
        {
            return FixedColors[existingItemCount];
        }

        return GenerateFallbackColor(usedColors);
    }

    private Color GenerateFallbackColor(IList<Color> usedColors)
    {
        var attemptCount = 0;
        var candidate = default(Color);

        do
        {
            _hueStep = (_hueStep + GoldenRatioConjugate) % 1.0;
            candidate = CreateColorFromHsv(_hueStep * 360.0, DefaultSaturation, DefaultBrightness);
            attemptCount++;
        }
        while (attemptCount < MaxRetryAttempts && IsTooSimilar(candidate, usedColors));

        return candidate;
    }

    private static int GetExistingItemCount(IEnumerable<IColoredItem>? existingItems)
    {
        if (existingItems is null)
        {
            return 0;
        }

        var count = 0;

        foreach (var item in existingItems)
        {
            count++;
        }

        return count;
    }

    private static List<Color> GetUsedColors(IEnumerable<IColoredItem>? existingItems)
    {
        var usedColors = new List<Color>();

        if (existingItems is null)
        {
            return usedColors;
        }

        foreach (var item in existingItems)
        {
            if (item is null)
            {
                continue;
            }

            if (!IsEmpty(item.Color))
            {
                usedColors.Add(item.Color);
            }
        }

        return usedColors;
    }

    private static bool IsTooSimilar(Color candidate, IList<Color> usedColors)
    {
        return GetMinimumDistance(candidate, usedColors) < ColorSimilarityThreshold;
    }

    private static int GetMinimumDistance(Color candidate, IList<Color> usedColors)
    {
        if (usedColors.Count == 0)
        {
            return int.MaxValue;
        }

        var minimumDistance = int.MaxValue;

        foreach (var usedColor in usedColors)
        {
            var distance = GetDistance(candidate, usedColor);

            if (distance < minimumDistance)
            {
                minimumDistance = distance;
            }
        }

        return minimumDistance;
    }

    private static int GetDistance(Color left, Color right)
    {
        return
            System.Math.Abs(left.R - right.R) +
            System.Math.Abs(left.G - right.G) +
            System.Math.Abs(left.B - right.B);
    }

    private static bool IsEmpty(Color color)
    {
        return color == default(Color);
    }

    private static Color CreateColorFromHsv(double hue, double saturation, double brightness)
    {
        var segment = Convert.ToInt32(System.Math.Floor(hue / 60.0)) % 6;
        var fractional = hue / 60.0 - System.Math.Floor(hue / 60.0);

        var value = ToByte(brightness * 255.0);
        var p = ToByte(brightness * 255.0 * (1.0 - saturation));
        var q = ToByte(brightness * 255.0 * (1.0 - fractional * saturation));
        var t = ToByte(brightness * 255.0 * (1.0 - (1.0 - fractional) * saturation));

        switch (segment)
        {
            case 0:
                return Color.FromArgb(255, value, t, p);
            case 1:
                return Color.FromArgb(255, q, value, p);
            case 2:
                return Color.FromArgb(255, p, value, t);
            case 3:
                return Color.FromArgb(255, p, q, value);
            case 4:
                return Color.FromArgb(255, t, p, value);
            default:
                return Color.FromArgb(255, value, p, q);
        }
    }

    private static byte ToByte(double value)
    {
        return Convert.ToByte(Convert.ToInt32(value));
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
