namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public interface IComparisonFileWriter
{
    public void WriteAllText(string filePath, string content);
}
