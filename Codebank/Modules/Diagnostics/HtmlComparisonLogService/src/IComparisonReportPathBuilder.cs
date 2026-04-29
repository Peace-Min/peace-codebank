using System;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public interface IComparisonReportPathBuilder
{
    public string BuildPath(ComparisonHtmlLogOptions options, DateTime createdAt);
}
