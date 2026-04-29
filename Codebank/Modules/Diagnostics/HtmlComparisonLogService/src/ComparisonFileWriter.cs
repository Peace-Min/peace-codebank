using System;
using System.IO;
using System.Text;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ComparisonFileWriter : IComparisonFileWriter
{
    private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);

    public void WriteAllText(string filePath, string content)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("filePath is required.", nameof(filePath));
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, content ?? string.Empty, Utf8Encoding);
    }
}
