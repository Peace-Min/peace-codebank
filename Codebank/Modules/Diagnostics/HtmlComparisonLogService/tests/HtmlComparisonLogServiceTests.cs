using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Peace.Codebank.Diagnostics.HtmlComparisonLogging;

namespace Peace.Codebank.Tests.Diagnostics.HtmlComparisonLogging;

public class HtmlComparisonLogServiceTests
{
    private static readonly string[] AlphaItemsAandB = { "A", "B" };
    private static readonly string[] AlphaItemsAandC = { "A", "C" };
    private static readonly string[] AlphaItemsAOnly = { "A" };

    [Fact]
    public void ComparisonReportBuilderBuildsEqualRowsFromNormalizedDatasets()
    {
        var builder = new ComparisonReportBuilder();
        var source = new ComparisonDataset
        {
            Objects = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Alpha"] = new List<string> { "B", "a", "A", string.Empty },
            },
        };
        var target = new ComparisonDataset
        {
            Objects = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Alpha"] = new List<string> { "a", "c" },
                ["Beta"] = new List<string> { "z" },
            },
        };
        var options = new ComparisonHtmlLogOptions
        {
            Mode = ComparisonMode.Equal,
        };

        var report = builder.Build(source, target, options);

        report.Title.Should().Be("비교 보고서");
        report.SourceLabel.Should().Be("소스");
        report.TargetLabel.Should().Be("타겟");
        report.Rows.Should().HaveCount(2);
        report.Rows[0].SourceKey.Should().Be("Alpha");
        report.Rows[0].TargetKey.Should().Be("Alpha");
        report.Rows[0].SourceItems.Should().Equal("a", "B");
        report.Rows[0].TargetItems.Should().Equal("a", "c");
        report.Rows[0].MissingItems.Should().Equal("c");
        report.Rows[0].ExtraItems.Should().Equal("B");
        report.Rows[0].IsMatch.Should().BeFalse();
        report.Rows[1].SourceKey.Should().BeNull();
        report.Rows[1].TargetKey.Should().Be("Beta");
        report.Rows[1].MissingItems.Should().Equal("z");
        report.IsMatch.Should().BeFalse();
    }

    [Fact]
    public void ComparisonReportBuilderSupportsSourceInTargetMode()
    {
        var builder = new ComparisonReportBuilder();
        var source = CreateDataset(("Alpha", AlphaItemsAandB));
        var target = CreateDataset(("Alpha", AlphaItemsAandC));
        var options = new ComparisonHtmlLogOptions
        {
            Mode = ComparisonMode.SourceInTarget,
        };

        var report = builder.Build(source, target, options);

        report.Rows.Should().HaveCount(1);
        report.Rows[0].SourceKey.Should().Be("Alpha");
        report.Rows[0].TargetKey.Should().Be("Alpha");
        report.Rows[0].MissingItems.Should().Equal("B");
        report.Rows[0].ExtraItems.Should().BeEmpty();
    }

    [Fact]
    public void ComparisonReportBuilderSupportsTargetInSourceMode()
    {
        var builder = new ComparisonReportBuilder();
        var source = CreateDataset(("Alpha", AlphaItemsAandB));
        var target = CreateDataset(("Alpha", AlphaItemsAandC));
        var options = new ComparisonHtmlLogOptions
        {
            Mode = ComparisonMode.TargetInSource,
        };

        var report = builder.Build(source, target, options);

        report.Rows.Should().HaveCount(1);
        report.Rows[0].SourceKey.Should().Be("Alpha");
        report.Rows[0].TargetKey.Should().Be("Alpha");
        report.Rows[0].MissingItems.Should().Equal("C");
        report.Rows[0].ExtraItems.Should().BeEmpty();
    }

    [Fact]
    public void ComparisonHtmlRendererRendersSectionsAndEscapesContent()
    {
        var renderer = new ComparisonHtmlRenderer();
        var report = new ComparisonReport
        {
            CreatedAt = new DateTime(2026, 4, 29, 10, 30, 0),
            Title = "비교 <보고서>",
            Mode = ComparisonMode.Equal,
            SourceLabel = "원본",
            TargetLabel = "대상",
            SourceDataset = CreateDataset(("<Alpha>", AlphaItemsAandB)),
            TargetDataset = CreateDataset(("<Alpha>", AlphaItemsAandC)),
            Rows = new List<ComparisonRow>
            {
                new ComparisonRow
                {
                    SourceKey = "<Alpha>",
                    TargetKey = "<Alpha>",
                    SourceItems = new List<string> { "A", "B" },
                    TargetItems = new List<string> { "A", "C" },
                    MissingItems = new List<string> { "C" },
                    ExtraItems = new List<string> { "B" },
                    IsMatch = false,
                },
            },
            IsMatch = false,
        };

        var html = renderer.Render(report);

        html.Should().Contain("<meta charset=\"utf-8\" />");
        html.Should().Contain("function openDetailFromHash()");
        html.Should().Contain("1. 원본 요약 표");
        html.Should().Contain("3. 비교 요약 표");
        html.Should().Contain("4. 상세 펼침 영역");
        html.Should().Contain("href=\"#detail-alpha-0\"");
        html.Should().Contain("&lt;Alpha&gt;");
        html.Should().Contain("대상 전용: C");
        html.Should().Contain("원본 전용: B");
    }

    [Fact]
    public void ComparisonReportPathBuilderBuildsSafeHtmlPath()
    {
        var builder = new ComparisonReportPathBuilder();
        var options = new ComparisonHtmlLogOptions
        {
            Title = "bad:name?",
        };

        var path = builder.BuildPath(options, new DateTime(2026, 4, 29, 11, 5, 7, 123));

        Path.GetFileName(path).Should().Be("bad_name__20260429_110507_123.html");
        path.Should().Contain("HtmlComparisonLogs");
    }

    [Fact]
    public void ComparisonFileWriterCreatesDirectoryAndWritesUtf8WithoutBom()
    {
        var writer = new ComparisonFileWriter();
        var rootPath = Path.Combine(Path.GetTempPath(), "peace-codebank-tests", Guid.NewGuid().ToString("N"));
        var filePath = Path.Combine(rootPath, "nested", "report.html");

        try
        {
            writer.WriteAllText(filePath, "가나다");

            File.Exists(filePath).Should().BeTrue();
            var bytes = File.ReadAllBytes(filePath);
            bytes.Length.Should().BeGreaterThan(0);
            bytes[0].Should().NotBe((byte)0xEF);
            File.ReadAllText(filePath, Encoding.UTF8).Should().Be("가나다");
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
        }
    }

    [Fact]
    public void ComparisonHtmlLogServiceReturnsFalseInsteadOfThrowing()
    {
        var failureSink = new RecordingFailureSink();
        var service = new ComparisonHtmlLogService(
            new ComparisonReportBuilder(),
            new ThrowingRenderer(),
            new ComparisonReportPathBuilder(),
            new RecordingFileWriter(),
            failureSink);

        var success = service.TryWriteHtmlReport(
            new ComparisonDataset(),
            new ComparisonDataset(),
            new ComparisonHtmlLogOptions(),
            out var reportPath);

        success.Should().BeFalse();
        reportPath.Should().BeNull();
        failureSink.Exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void ComparisonHtmlLogServiceDelegatesToSeparatedDependencies()
    {
        var writer = new RecordingFileWriter();
        var service = new ComparisonHtmlLogService(
            new ComparisonReportBuilder(),
            new ComparisonHtmlRenderer(),
            new ComparisonReportPathBuilder(),
            writer,
            new RecordingFailureSink());

        var success = service.TryWriteHtmlReport(
            CreateDataset(("Alpha", AlphaItemsAOnly)),
            CreateDataset(("Alpha", AlphaItemsAOnly)),
            new ComparisonHtmlLogOptions
            {
                OutputDirectory = Path.Combine(Path.GetTempPath(), "comparison-log-service-tests"),
                Title = "테스트",
                SourceLabel = "왼쪽",
                TargetLabel = "오른쪽",
                Mode = ComparisonMode.Equal,
            },
            out var reportPath);

        success.Should().BeTrue();
        reportPath.Should().NotBeNullOrWhiteSpace();
        writer.FilePath.Should().Be(reportPath);
        writer.Content.Should().Contain("<!DOCTYPE html>");
        writer.Content.Should().Contain("왼쪽");
        writer.Content.Should().Contain("오른쪽");
    }

    [Fact]
    public void ComparisonHtmlLogServiceWritesHtmlArtifactToTestResults()
    {
        var outputDirectory = GetArtifactDirectory();
        var service = new ComparisonHtmlLogService();

        var success = service.TryWriteHtmlReport(
            CreateDataset(("Alpha", AlphaItemsAandB)),
            CreateDataset(("Alpha", AlphaItemsAandC)),
            new ComparisonHtmlLogOptions
            {
                OutputDirectory = outputDirectory,
                Title = "HtmlComparisonLogService_TestArtifact",
                SourceLabel = "Source",
                TargetLabel = "Target",
                Mode = ComparisonMode.Equal,
            },
            out var reportPath);

        success.Should().BeTrue();
        reportPath.Should().NotBeNullOrWhiteSpace();
        reportPath!.StartsWith(outputDirectory, StringComparison.OrdinalIgnoreCase).Should().BeTrue();
        File.Exists(reportPath).Should().BeTrue();
        File.ReadAllText(reportPath!).Should().Contain("<!DOCTYPE html>");
    }

    private static ComparisonDataset CreateDataset(params (string Key, string[] Items)[] entries)
    {
        var dataset = new ComparisonDataset();

        foreach (var entry in entries)
        {
            dataset.Objects[entry.Key] = new List<string>(entry.Items);
        }

        return dataset;
    }

    private static string GetArtifactDirectory()
    {
        var repositoryRoot = FindRepositoryRoot();
        var frameworkName = SanitizePathSegment(AppContext.TargetFrameworkName ?? "unknown");
        return Path.Combine(repositoryRoot, "TestResults", "HtmlComparisonLogService", frameworkName);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Peace.Codebank.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root was not found.");
    }

    private static string SanitizePathSegment(string value)
    {
        var buffer = value.ToCharArray();
        var invalidChars = Path.GetInvalidFileNameChars();

        for (var i = 0; i < buffer.Length; i++)
        {
            if (Array.IndexOf(invalidChars, buffer[i]) >= 0)
            {
                buffer[i] = '_';
                continue;
            }

            if (buffer[i] == ',' || buffer[i] == '=' || char.IsWhiteSpace(buffer[i]))
            {
                buffer[i] = '_';
            }
        }

        return new string(buffer);
    }

    private sealed class ThrowingRenderer : IComparisonHtmlRenderer
    {
        public string Render(ComparisonReport report)
        {
            throw new InvalidOperationException("render failed");
        }
    }

    private sealed class RecordingFileWriter : IComparisonFileWriter
    {
        public string? Content { get; private set; }

        public string? FilePath { get; private set; }

        public void WriteAllText(string filePath, string content)
        {
            FilePath = filePath;
            Content = content;
        }
    }

    private sealed class RecordingFailureSink : IComparisonFailureSink
    {
        public Exception? Exception { get; private set; }

        public void Report(Exception exception)
        {
            Exception = exception;
        }
    }
}
