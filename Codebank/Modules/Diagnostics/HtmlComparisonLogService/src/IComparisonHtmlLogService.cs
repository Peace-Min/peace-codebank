namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public interface IComparisonHtmlLogService
{
    public bool TryWriteHtmlReport(
        ComparisonDataset sourceDataset,
        ComparisonDataset targetDataset,
        ComparisonHtmlLogOptions options,
        out string? reportPath);
}
