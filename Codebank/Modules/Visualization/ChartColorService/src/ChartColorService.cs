using System;
using System.Collections.Generic;
using System.Drawing;

namespace Peace.Codebank.Visualization.Charting;

public sealed class ChartColorService
{
    private const double GoldenRatioConjugate = 0.618033988749895;
    private const int ColorSimilarityThreshold = 75;
    private const int MaxRetryAttempts = 10;
    private const double DefaultSaturation = 0.8;
    private const double DefaultBrightness = 0.6;

    private double _hueStep;

    public Color GenerateUniqueColor(IEnumerable<IColoredItem>? existingItems)
    {
        var usedColors = GetUsedColors(existingItems);
        var attemptCount = 0;
        var candidate = Color.Empty;

        do
        {
            _hueStep = (_hueStep + GoldenRatioConjugate) % 1.0;
            candidate = CreateColorFromHsv(_hueStep * 360.0, DefaultSaturation, DefaultBrightness);
            attemptCount++;
        }
        while (attemptCount < MaxRetryAttempts && IsTooSimilar(candidate, usedColors));

        return candidate;
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

            if (!item.DisplayColor.IsEmpty)
            {
                usedColors.Add(item.DisplayColor);
            }
        }

        return usedColors;
    }

    private static bool IsTooSimilar(Color candidate, IList<Color> usedColors)
    {
        if (usedColors.Count == 0)
        {
            return false;
        }

        foreach (var usedColor in usedColors)
        {
            var distance =
                System.Math.Abs(usedColor.R - candidate.R) +
                System.Math.Abs(usedColor.G - candidate.G) +
                System.Math.Abs(usedColor.B - candidate.B);

            if (distance < ColorSimilarityThreshold)
            {
                return true;
            }
        }

        return false;
    }

    private static Color CreateColorFromHsv(double hue, double saturation, double brightness)
    {
        var segment = Convert.ToInt32(System.Math.Floor(hue / 60.0)) % 6;
        var fractional = hue / 60.0 - System.Math.Floor(hue / 60.0);

        var value = Convert.ToInt32(brightness * 255.0);
        var p = Convert.ToInt32(brightness * 255.0 * (1.0 - saturation));
        var q = Convert.ToInt32(brightness * 255.0 * (1.0 - fractional * saturation));
        var t = Convert.ToInt32(brightness * 255.0 * (1.0 - (1.0 - fractional) * saturation));

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
}
