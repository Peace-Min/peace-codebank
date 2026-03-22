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
    private static readonly Color FirstRegisteredColor = Color.FromArgb(255, 242, 24, 24);
    private static readonly Color[] PreferredColors =
    {
        FirstRegisteredColor,
        CreateColorFromHsv(28.0, 0.82, 0.96),
        CreateColorFromHsv(48.0, 0.80, 0.95),
        CreateColorFromHsv(84.0, 0.68, 0.84),
        CreateColorFromHsv(124.0, 0.68, 0.73),
        CreateColorFromHsv(164.0, 0.74, 0.78),
        CreateColorFromHsv(196.0, 0.78, 0.90),
        CreateColorFromHsv(224.0, 0.72, 0.92),
        CreateColorFromHsv(256.0, 0.66, 0.86),
        CreateColorFromHsv(288.0, 0.69, 0.84),
        CreateColorFromHsv(326.0, 0.76, 0.90),
    };

    private double _hueStep;

    public Color GenerateUniqueColor(IEnumerable<IColoredItem>? existingItems)
    {
        var usedColors = GetUsedColors(existingItems);

        if (usedColors.Count == 0)
        {
            return FirstRegisteredColor;
        }

        var preferredColor = FindPreferredColor(usedColors);

        if (!IsEmpty(preferredColor))
        {
            return preferredColor;
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

    private static Color FindPreferredColor(IList<Color> usedColors)
    {
        var bestCandidate = default(Color);
        var bestScore = int.MinValue;

        foreach (var candidate in PreferredColors)
        {
            if (ContainsExactColor(candidate, usedColors))
            {
                continue;
            }

            var score = GetMinimumDistance(candidate, usedColors);

            if (score > bestScore)
            {
                bestCandidate = candidate;
                bestScore = score;
            }
        }

        return bestCandidate;
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

    private static bool ContainsExactColor(Color candidate, IList<Color> usedColors)
    {
        foreach (var usedColor in usedColors)
        {
            if (ToArgb(usedColor) == ToArgb(candidate))
            {
                return true;
            }
        }

        return false;
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
