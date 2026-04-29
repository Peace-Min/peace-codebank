using System;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public interface IComparisonFailureSink
{
    public void Report(Exception exception);
}
