using System.Collections.Generic;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ComparisonRow
{
    public string? SourceKey { get; set; }

    public string? TargetKey { get; set; }

    public List<string> SourceItems { get; set; } = new List<string>();

    public List<string> TargetItems { get; set; } = new List<string>();

    public List<string> MissingItems { get; set; } = new List<string>();

    public List<string> ExtraItems { get; set; } = new List<string>();

    public bool IsMatch { get; set; }
}
