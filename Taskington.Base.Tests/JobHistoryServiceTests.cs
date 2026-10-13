using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Taskington.Base.Models;
using Taskington.Base.Platform;
using Taskington.Base.Storage;
using Xunit;

namespace Taskington.Base.Tests;

public class JobHistoryServiceTests
{
    private const string TestHome = "/taskington-test-home";

    private static string HistoryDirectory => Path.Combine(
        TestHome, "Library", "Application Support", "Taskington", "history");

    private static (JobHistoryService Service, InMemoryFileSystem FileSystem) CreateService()
    {
        var fileSystem = new InMemoryFileSystem();
        var platformDirectories = new PlatformDirectoryService(
            DesktopPlatform.MacOS, _ => null, TestHome);
        return (new JobHistoryService(platformDirectories, fileSystem), fileSystem);
    }

    private static ExecutionResult CreateResult(
        JobState status = JobState.Completed,
        ExecutionMode mode = ExecutionMode.Normal,
        DateTimeOffset? startTime = null,
        int? exitCode = 0)
    {
        return new ExecutionResult(
            Guid.NewGuid(), Guid.NewGuid(), mode, status, startTime ?? DateTimeOffset.Now,
            (startTime ?? DateTimeOffset.Now).AddMinutes(1), exitCode, "failure details");
    }

    [Fact]
    public void RecordExecutionWritesMetadataFileUnderJobHistoryDirectory()
    {
        var (service, fileSystem) = CreateService();
        var result = CreateResult();

        service.RecordExecution(result);

        string file = Path.Combine(HistoryDirectory, result.JobId.ToString(), result.ExecutionId + ".json");
        Assert.True(fileSystem.FileExists(file));
        using var document = JsonDocument.Parse(fileSystem.ReadAllText(file));
        var properties = document.RootElement.EnumerateObject().Select(p => p.Name).ToList();
        Assert.Equal(
            new[]
            {
                "executionId", "jobId", "startTime", "endTime",
                "executionMode", "status", "exitCode"
            },
            properties);
        Assert.Equal(result.JobId.ToString(), document.RootElement.GetProperty("jobId").GetString());
        Assert.Equal("normal", document.RootElement.GetProperty("executionMode").GetString());
        Assert.Equal("completed", document.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public void RecordedMetadataNeverContainsOutputOrErrorMessage()
    {
        var (service, fileSystem) = CreateService();
        var result = CreateResult(status: JobState.Failed, exitCode: 1);

        service.RecordExecution(result);

        string json = fileSystem.ReadAllText(
            Path.Combine(HistoryDirectory, result.JobId.ToString(), result.ExecutionId + ".json"));
        Assert.DoesNotContain("errorMessage", json);
        Assert.DoesNotContain("failure details", json);
        Assert.DoesNotContain("output", json);
    }

    [Fact]
    public void LoadHistoryReturnsRecordedExecutionsWithAllValues()
    {
        var (service, _) = CreateService();
        var result = CreateResult(mode: ExecutionMode.Dry, exitCode: 3);
        service.RecordExecution(result);

        var history = service.LoadHistory(result.JobId);

        var loaded = Assert.Single(history);
        Assert.Equal(result.ExecutionId, loaded.ExecutionId);
        Assert.Equal(result.JobId, loaded.JobId);
        Assert.Equal(JobState.Completed, loaded.Status);
        Assert.Equal(ExecutionMode.Dry, loaded.ExecutionMode);
        Assert.Equal(result.StartTime, loaded.StartTime);
        Assert.Equal(result.EndTime, loaded.EndTime);
        Assert.Equal(3, loaded.ExitCode);
    }

    [Fact]
    public void LoadHistoryReturnsEmptyForUnknownJob()
    {
        var (service, _) = CreateService();

        Assert.Empty(service.LoadHistory(Guid.NewGuid()));
    }

    [Fact]
    public void LoadHistoryReturnsExecutionsOldestFirst()
    {
        var (service, _) = CreateService();
        var jobId = Guid.NewGuid();
        var now = DateTimeOffset.Now;
        var oldest = new ExecutionResult(
            Guid.NewGuid(), jobId, ExecutionMode.Normal, JobState.Completed, now.AddHours(-2), now.AddHours(-2).AddMinutes(1), 0, null);
        var middle = new ExecutionResult(
            Guid.NewGuid(), jobId, ExecutionMode.Normal, JobState.Failed, now.AddHours(-1), now.AddHours(-1).AddMinutes(1), 1, "err");
        var newest = new ExecutionResult(
            Guid.NewGuid(), jobId, ExecutionMode.Normal, JobState.Cancelled, now, now.AddMinutes(1), null, null);
        service.RecordExecution(newest);
        service.RecordExecution(oldest);
        service.RecordExecution(middle);

        var history = service.LoadHistory(jobId);

        Assert.Equal(3, history.Count);
        Assert.Equal(oldest.ExecutionId, history[0].ExecutionId);
        Assert.Equal(middle.ExecutionId, history[1].ExecutionId);
        Assert.Equal(newest.ExecutionId, history[2].ExecutionId);
    }

    [Fact]
    public void RecordExecutionOverwritesRecordOfSameExecution()
    {
        var (service, _) = CreateService();
        var result = CreateResult(exitCode: 0);
        service.RecordExecution(result);

        var corrected = new ExecutionResult(
            result.ExecutionId, result.JobId, result.ExecutionMode, JobState.Failed,
            result.StartTime, result.EndTime, 1, "exit code arrived late");
        service.RecordExecution(corrected);

        var history = service.LoadHistory(result.JobId);
        var loaded = Assert.Single(history);
        Assert.Equal(JobState.Failed, loaded.Status);
        Assert.Equal(1, loaded.ExitCode);
    }

    [Fact]
    public void LoadHistorySkipsMalformedFiles()
    {
        var (service, fileSystem) = CreateService();
        var result = CreateResult();
        service.RecordExecution(result);
        string jobDirectory = Path.Combine(HistoryDirectory, result.JobId.ToString());
        fileSystem.WriteAllText(Path.Combine(jobDirectory, Guid.NewGuid() + ".json"), "not JSON");

        var history = service.LoadHistory(result.JobId);

        var loaded = Assert.Single(history);
        Assert.Equal(result.ExecutionId, loaded.ExecutionId);
    }
}
