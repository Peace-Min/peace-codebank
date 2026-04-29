using System;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ConsoleComparisonFailureSink : IComparisonFailureSink
{
    public void Report(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        Console.Error.WriteLine("[HtmlComparisonLogService] " + exception);
    }
}
