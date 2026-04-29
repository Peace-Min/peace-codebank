using System;
using System.Collections.Generic;
using System.Linq;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ComparisonReportBuilder : IComparisonReportBuilder
{
    public ComparisonReport Build(
        ComparisonDataset sourceDataset,
        ComparisonDataset targetDataset,
        ComparisonHtmlLogOptions options)
    {
        if (sourceDataset == null)
        {
            throw new ArgumentNullException(nameof(sourceDataset));
        }

        if (targetDataset == null)
        {
            throw new ArgumentNullException(nameof(targetDataset));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var normalizedSource = NormalizeDataset(sourceDataset);
        var normalizedTarget = NormalizeDataset(targetDataset);
        var report = new ComparisonReport
        {
            CreatedAt = DateTime.Now,
            Title = ResolveText(options.Title, "비교 보고서"),
            Mode = options.Mode,
            SourceLabel = ResolveText(options.SourceLabel, "소스"),
            TargetLabel = ResolveText(options.TargetLabel, "타겟"),
            SourceDataset = normalizedSource,
            TargetDataset = normalizedTarget,
        };

        report.Rows = BuildRows(normalizedSource, normalizedTarget, options.Mode);
        report.IsMatch = report.Rows.All(row => row.IsMatch);

        return report;
    }

    private static ComparisonDataset NormalizeDataset(ComparisonDataset dataset)
    {
        var normalized = new ComparisonDataset();

        if (dataset.Objects == null)
        {
            return normalized;
        }

        foreach (var pair in dataset.Objects.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
        {
            var key = pair.Key ?? string.Empty;
            var normalizedItems = (pair.Value ?? new List<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToList();

            normalized.Objects[key] = normalizedItems;
        }

        return normalized;
    }

    private static List<ComparisonRow> BuildRows(
        ComparisonDataset sourceDataset,
        ComparisonDataset targetDataset,
        ComparisonMode mode)
    {
        switch (mode)
        {
            case ComparisonMode.SourceInTarget:
                return BuildSourceInTargetRows(sourceDataset, targetDataset);
            case ComparisonMode.TargetInSource:
                return BuildTargetInSourceRows(sourceDataset, targetDataset);
            case ComparisonMode.Equal:
            default:
                return BuildEqualRows(sourceDataset, targetDataset);
        }
    }

    private static List<ComparisonRow> BuildSourceInTargetRows(
        ComparisonDataset sourceDataset,
        ComparisonDataset targetDataset)
    {
        var rows = new List<ComparisonRow>();

        foreach (var pair in sourceDataset.Objects)
        {
            var sourceKey = pair.Key;
            var sourceItems = pair.Value;
            var targetItems = targetDataset.Objects.TryGetValue(sourceKey, out var matchedTargetItems)
                ? matchedTargetItems
                : new List<string>();
            var missingItems = sourceItems
                .Where(item => !targetItems.Contains(item, StringComparer.OrdinalIgnoreCase))
                .ToList();

            rows.Add(new ComparisonRow
            {
                SourceKey = sourceKey,
                TargetKey = targetDataset.Objects.ContainsKey(sourceKey) ? sourceKey : null,
                SourceItems = sourceItems,
                TargetItems = targetItems,
                MissingItems = missingItems,
                IsMatch = missingItems.Count == 0,
            });
        }

        return rows;
    }

    private static List<ComparisonRow> BuildTargetInSourceRows(
        ComparisonDataset sourceDataset,
        ComparisonDataset targetDataset)
    {
        var rows = new List<ComparisonRow>();

        foreach (var pair in targetDataset.Objects)
        {
            var targetKey = pair.Key;
            var targetItems = pair.Value;
            var sourceItems = sourceDataset.Objects.TryGetValue(targetKey, out var matchedSourceItems)
                ? matchedSourceItems
                : new List<string>();
            var missingItems = targetItems
                .Where(item => !sourceItems.Contains(item, StringComparer.OrdinalIgnoreCase))
                .ToList();

            rows.Add(new ComparisonRow
            {
                SourceKey = sourceDataset.Objects.ContainsKey(targetKey) ? targetKey : null,
                TargetKey = targetKey,
                SourceItems = sourceItems,
                TargetItems = targetItems,
                MissingItems = missingItems,
                IsMatch = missingItems.Count == 0,
            });
        }

        return rows;
    }

    private static List<ComparisonRow> BuildEqualRows(
        ComparisonDataset sourceDataset,
        ComparisonDataset targetDataset)
    {
        var keys = sourceDataset.Objects.Keys
            .Union(targetDataset.Objects.Keys, StringComparer.OrdinalIgnoreCase)
            .OrderBy(key => key, StringComparer.OrdinalIgnoreCase);
        var rows = new List<ComparisonRow>();

        foreach (var key in keys)
        {
            var sourceItems = sourceDataset.Objects.TryGetValue(key, out var matchedSourceItems)
                ? matchedSourceItems
                : new List<string>();
            var targetItems = targetDataset.Objects.TryGetValue(key, out var matchedTargetItems)
                ? matchedTargetItems
                : new List<string>();
            var missingItems = targetItems
                .Where(item => !sourceItems.Contains(item, StringComparer.OrdinalIgnoreCase))
                .ToList();
            var extraItems = sourceItems
                .Where(item => !targetItems.Contains(item, StringComparer.OrdinalIgnoreCase))
                .ToList();

            rows.Add(new ComparisonRow
            {
                SourceKey = sourceDataset.Objects.ContainsKey(key) ? key : null,
                TargetKey = targetDataset.Objects.ContainsKey(key) ? key : null,
                SourceItems = sourceItems,
                TargetItems = targetItems,
                MissingItems = missingItems,
                ExtraItems = extraItems,
                IsMatch = missingItems.Count == 0 && extraItems.Count == 0,
            });
        }

        return rows;
    }

    private static string ResolveText(string? value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value!;
    }
}
