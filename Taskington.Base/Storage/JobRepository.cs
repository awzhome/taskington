using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Serilog;
using Taskington.Base.FileSystem;
using Taskington.Base.Models;
using Taskington.Base.Platform;

namespace Taskington.Base.Storage;

/// <summary>
/// Stores jobs as one JSON file per job under the platform jobs/ directory
/// (spec chapter 8): the job id GUID is the file name. Writes are atomic via a
/// unique temp file plus rename, so readers always see a complete file even
/// under concurrent saves. Malformed files are skipped with a log entry instead
/// of failing the whole load.
/// </summary>
public sealed class JobRepository(
    PlatformDirectoryService platformDirectories,
    IFileSystemService fileSystem)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    private string JobsDirectory => platformDirectories.JobsDirectory;

    /// <summary>
    /// All persisted jobs; malformed files are skipped.
    /// </summary>
    public IReadOnlyList<Job> LoadAll()
    {
        fileSystem.CreateDirectory(JobsDirectory);
        return fileSystem.GetFiles(JobsDirectory, "*.json")
            .Select(LoadFromFile)
            .Where(job => job is not null)
            .Select(job => job!)
            .ToList();
    }

    /// <summary>
    /// The persisted job with the given id, or null if there is none.
    /// </summary>
    public Job? Load(Guid id)
    {
        fileSystem.CreateDirectory(JobsDirectory);
        return fileSystem.FileExists(GetJobFilePath(id)) ? LoadFromFile(GetJobFilePath(id)) : null;
    }

    /// <summary>
    /// Persists the job: assigns the id and CreatedAt on the first save and
    /// updates ModifiedAt on every save. The job instance is updated in place.
    /// </summary>
    public void Save(Job job)
    {
        if (job.Id == Guid.Empty)
        {
            job.Id = Guid.NewGuid();
        }

        var now = DateTimeOffset.Now;
        if (job.CreatedAt == default)
        {
            job.CreatedAt = now;
        }

        job.ModifiedAt = now;

        fileSystem.CreateDirectory(JobsDirectory);
        string targetFile = GetJobFilePath(job.Id);
        string tempFile = Path.Combine(JobsDirectory, $"{job.Id:N}-{Guid.NewGuid():N}.tmp");
        fileSystem.WriteAllText(tempFile, JsonSerializer.Serialize(ToJobFile(job), JsonOptions));
        fileSystem.MoveFile(tempFile, targetFile, overwrite: true);
    }

    /// <summary>
    /// Deletes the job's file; true if a file was removed, false if the job
    /// does not exist. Job history is not touched (removed with the job by the
    /// UI flow, spec chapter 5).
    /// </summary>
    public bool Delete(Guid id)
    {
        if (!fileSystem.FileExists(GetJobFilePath(id)))
        {
            return false;
        }

        fileSystem.DeleteFile(GetJobFilePath(id));
        return true;
    }

    private Job? LoadFromFile(string path)
    {
        try
        {
            var jobFile = JsonSerializer.Deserialize<JobFile>(fileSystem.ReadAllText(path), JsonOptions);
            return jobFile is null || !IsComplete(jobFile) ? null : ToJob(jobFile);
        }
        catch (JsonException exception)
        {
            Log.Warning(exception, "Skipping malformed job file {JobFile}", path);
            return null;
        }
    }

    /// <summary>
    /// System.Text.Json fills missing constructor parameters with default values,
    /// so a file that lacks required fields is only recognizable by its defaults.
    /// </summary>
    private static bool IsComplete(JobFile jobFile)
    {
        return jobFile.Id != Guid.Empty
            && jobFile.Name is not null
            && jobFile.Description is not null
            && jobFile.Script is not null;
    }

    private string GetJobFilePath(Guid id)
    {
        return Path.Combine(JobsDirectory, $"{id}.json");
    }

    private static JobFile ToJobFile(Job job)
    {
        return new JobFile(job.Id, job.Name, job.Description, job.Script, job.CreatedAt, job.ModifiedAt);
    }

    private static Job ToJob(JobFile jobFile)
    {
        return new Job
        {
            Id = jobFile.Id,
            Name = jobFile.Name,
            Description = jobFile.Description,
            Script = jobFile.Script,
            CreatedAt = jobFile.CreatedAt,
            ModifiedAt = jobFile.ModifiedAt,
            IsTemporary = false
        };
    }

    /// <summary>
    /// Storage shape of a job file (spec chapter 8): exactly the persisted
    /// fields; a temporary job is never persisted as temporary.
    /// </summary>
    private sealed record JobFile(
        Guid Id,
        string Name,
        string Description,
        string Script,
        DateTimeOffset CreatedAt,
        DateTimeOffset ModifiedAt);
}
