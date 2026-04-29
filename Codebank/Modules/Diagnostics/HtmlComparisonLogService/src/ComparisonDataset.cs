using System;
using System.Collections.Generic;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ComparisonDataset
{
    public ComparisonDataset()
    {
        Objects = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
    }

    public Dictionary<string, List<string>> Objects { get; set; }
}
