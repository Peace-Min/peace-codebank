using System;
using System.Collections.Generic;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ComparisonReport
{
    public DateTime CreatedAt { get; set; }

    public string Title { get; set; } = string.Empty;

    public ComparisonMode Mode { get; set; }

    public string SourceLabel { get; set; } = string.Empty;

    public string TargetLabel { get; set; } = string.Empty;

    public ComparisonDataset SourceDataset { get; set; } = new ComparisonDataset();

    public ComparisonDataset TargetDataset { get; set; } = new ComparisonDataset();

    public List<ComparisonRow> Rows { get; set; } = new List<ComparisonRow>();

    public bool IsMatch { get; set; }
}
