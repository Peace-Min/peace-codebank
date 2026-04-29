namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public interface IComparisonReportBuilder
{
    public ComparisonReport Build(
        ComparisonDataset sourceDataset,
        ComparisonDataset targetDataset,
        ComparisonHtmlLogOptions options);
}
