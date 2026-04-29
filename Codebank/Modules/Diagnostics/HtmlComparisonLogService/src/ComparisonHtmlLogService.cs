using System;

namespace Peace.Codebank.Diagnostics.HtmlComparisonLogging;

public sealed class ComparisonHtmlLogService : IComparisonHtmlLogService
{
    private readonly IComparisonReportBuilder _reportBuilder;
    private readonly IComparisonHtmlRenderer _htmlRenderer;
    private readonly IComparisonReportPathBuilder _pathBuilder;
    private readonly IComparisonFileWriter _fileWriter;
    private readonly IComparisonFailureSink _failureSink;

    public ComparisonHtmlLogService()
        : this(
            new ComparisonReportBuilder(),
            new ComparisonHtmlRenderer(),
            new ComparisonReportPathBuilder(),
            new ComparisonFileWriter(),
            new ConsoleComparisonFailureSink())
    {
    }

    public ComparisonHtmlLogService(
        IComparisonReportBuilder reportBuilder,
        IComparisonHtmlRenderer htmlRenderer,
        IComparisonReportPathBuilder pathBuilder,
        IComparisonFileWriter fileWriter,
        IComparisonFailureSink failureSink)
    {
        _reportBuilder = reportBuilder ?? throw new ArgumentNullException(nameof(reportBuilder));
        _htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
        _pathBuilder = pathBuilder ?? throw new ArgumentNullException(nameof(pathBuilder));
        _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        _failureSink = failureSink ?? throw new ArgumentNullException(nameof(failureSink));
    }

    public bool TryWriteHtmlReport(
        ComparisonDataset sourceDataset,
        ComparisonDataset targetDataset,
        ComparisonHtmlLogOptions options,
        out string? reportPath)
    {
        reportPath = null;

#pragma warning disable CA1031
        try
        {
            var report = _reportBuilder.Build(sourceDataset, targetDataset, options);
            var html = _htmlRenderer.Render(report);
            reportPath = _pathBuilder.BuildPath(options, report.CreatedAt);

            _fileWriter.WriteAllText(reportPath, html);
            return true;
        }
        catch (Exception exception)
        {
            // Fail-safe 진입점은 비교/렌더링/저장 실패를 false로 바꿉니다.
            try
            {
                _failureSink.Report(exception);
            }
            catch (Exception)
            {
                // 실패 보고 중 예외도 호출부까지 전파하지 않습니다.
            }

            reportPath = null;
            return false;
        }
#pragma warning restore CA1031
    }
}
