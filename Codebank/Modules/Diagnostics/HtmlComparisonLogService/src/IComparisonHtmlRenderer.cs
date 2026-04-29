namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public interface IComparisonHtmlRenderer
{
    public string Render(ComparisonReport report);
}
