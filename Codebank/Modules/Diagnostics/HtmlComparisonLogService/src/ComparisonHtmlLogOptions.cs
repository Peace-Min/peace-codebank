namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ComparisonHtmlLogOptions
{
    public string? OutputDirectory { get; set; }

    public string? Title { get; set; }

    public string? SourceLabel { get; set; }

    public string? TargetLabel { get; set; }

    public ComparisonMode Mode { get; set; }
}
