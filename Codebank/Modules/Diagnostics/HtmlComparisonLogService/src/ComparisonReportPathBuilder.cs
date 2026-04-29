using System;
using System.Globalization;
using System.IO;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ComparisonReportPathBuilder : IComparisonReportPathBuilder
{
    public string BuildPath(ComparisonHtmlLogOptions options, DateTime createdAt)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var outputDirectory = ResolveOutputDirectory(options.OutputDirectory);
        var fileName = BuildFileName(options.Title, createdAt);
        return Path.Combine(outputDirectory, fileName);
    }

    private static string ResolveOutputDirectory(string? outputDirectory)
    {
        if (!string.IsNullOrWhiteSpace(outputDirectory))
        {
            return outputDirectory!;
        }

        return Path.Combine(GetDefaultRootDirectory(), "HtmlComparisonLogs");
    }

    private static string BuildFileName(string? title, DateTime createdAt)
    {
        var safeTitle = SanitizeFileName(string.IsNullOrWhiteSpace(title) ? "비교_보고서" : title!);
        if (safeTitle.Length == 0)
        {
            safeTitle = "비교_보고서";
        }

        return safeTitle + "_" + createdAt.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture) + ".html";
    }

    private static string SanitizeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var buffer = value.ToCharArray();

        for (var i = 0; i < buffer.Length; i++)
        {
            if (Array.IndexOf(invalidChars, buffer[i]) >= 0)
            {
                buffer[i] = '_';
            }
        }

        return new string(buffer).Trim();
    }

    private static string GetDefaultRootDirectory()
    {
        var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (!string.IsNullOrWhiteSpace(localAppDataPath))
        {
            return localAppDataPath;
        }

        return Path.GetTempPath();
    }
}
