using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;
using Taskington.Base.FileSystem;
using Taskington.Base.Models;
using Taskington.Base.Platform;

namespace Taskington.Base.Storage;

/// <summary>
/// Stores execution metadata as one JSON file per execution under the platform
/// history/&lt;job id&gt;/ directory (spec chapter 8), recorded when an execution
/// completes. Metadata only - job output is never persisted. Malformed files are
/// skipped with a log entry instead of failing the whole load.
/// </summary>
public sealed class JobHistoryService(
    PlatformDirectoryService platformDirectories,
    IFileSystemService fileSystem)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        WriteIndented = true,
    };

    /// <summary>
    /// Writes the execution's metadata file, atomically via temp file and rename.
    /// Recording the same execution again overwrites its file.
    /// </summary>
    public void RecordExecution(ExecutionResult result)
    {
        string jobHistoryDirectory = GetJobHistoryDirectory(result.JobId);
        fileSystem.CreateDirectory(jobHistoryDirectory);

        string targetFile = Path.Combine(jobHistoryDirectory, $"{result.ExecutionId}.json");
        string tempFile = Path.Combine(
            jobHistoryDirectory, $"{result.ExecutionId:N}-{Guid.NewGuid():N}.tmp");
        fileSystem.WriteAllText(tempFile, JsonSerializer.Serialize(ToRecord(result), JsonOptions));
        fileSystem.MoveFile(tempFile, targetFile, overwrite: true);
    }

    /// <summary>
    /// All recorded executions of the given job, oldest first.
    /// </summary>
    public IReadOnlyList<ExecutionResult> LoadHistory(Guid jobId)
    {
        string jobHistoryDirectory = GetJobHistoryDirectory(jobId);
        fileSystem.CreateDirectory(jobHistoryDirectory);
        return fileSystem.GetFiles(jobHistoryDirectory, "*.json")
            .Select(LoadFromFile)
            .Where(result => result is not null)
            .Select(result => result!)
            .OrderBy(result => result.StartTime)
            .ToList();
    }

    private ExecutionResult? LoadFromFile(string path)
    {
        try
        {
            var record = JsonSerializer.Deserialize<ExecutionRecord>(fileSystem.ReadAllText(path), JsonOptions);
            return record is null || !IsComplete(record) ? null : ToResult(record);
        }
        catch (JsonException exception)
        {
            Log.Warning(exception, "Skipping malformed history file {HistoryFile}", path);
            return null;
        }
    }

    private string GetJobHistoryDirectory(Guid jobId)
    {
        return Path.Combine(platformDirectories.HistoryDirectory, jobId.ToString());
    }

    /// <summary>
    /// System.Text.Json fills missing constructor parameters with default values,
    /// so a file that lacks required fields is only recognizable by its defaults.
    /// </summary>
    private static bool IsComplete(ExecutionRecord record)
    {
        return record.ExecutionId != Guid.Empty
            && record.JobId != Guid.Empty;
    }

    private static ExecutionRecord ToRecord(ExecutionResult result)
    {
        return new ExecutionRecord(
            result.ExecutionId,
            result.JobId,
            result.StartTime,
            result.EndTime,
            result.ExecutionMode,
            result.Status,
            result.ExitCode);
    }

    private static ExecutionResult ToResult(ExecutionRecord record)
    {
        return new ExecutionResult(
            record.ExecutionId,
            record.JobId,
            record.ExecutionMode,
            record.Status,
            record.StartTime,
            record.EndTime,
            record.ExitCode,
            errorMessage: null);
    }

    /// <summary>
    /// Storage shape of a history file (spec chapter 8): exactly the persisted
    /// metadata fields, never output.
    /// </summary>
    private sealed record ExecutionRecord(
        Guid ExecutionId,
        Guid JobId,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime,
        ExecutionMode ExecutionMode,
        JobState Status,
        int? ExitCode);
}
