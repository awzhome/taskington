using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Taskington.Base.FileSystem;
using Taskington.Base.Models;
using Taskington.Base.Platform;
using Taskington.Base.Storage;
using Xunit;

namespace Taskington.Base.Tests;

public class JobRepositoryTests
{
    private const string TestHome = "/taskington-test-home";

    private static string JobsDirectory => Path.Combine(
        TestHome, "Library", "Application Support", "Taskington", "jobs");

    private static (JobRepository Repository, InMemoryFileSystem FileSystem) CreateRepository()
    {
        var fileSystem = new InMemoryFileSystem();
        var platformDirectories = new PlatformDirectoryService(
            DesktopPlatform.MacOS, _ => null, TestHome);
        return (new JobRepository(platformDirectories, fileSystem), fileSystem);
    }

    private static Job CreateJob(string name = "Test job")
    {
        return new Job
        {
            Name = name,
            Description = "A job for testing",
            Script = "log('hello')"
        };
    }

    [Fact]
    public void SaveAssignsIdAndTimestampsToNewJob()
    {
        var (repository, _) = CreateRepository();
        var job = CreateJob();
        var before = DateTimeOffset.Now;

        repository.Save(job);

        Assert.NotEqual(Guid.Empty, job.Id);
        Assert.InRange(job.CreatedAt, before, DateTimeOffset.Now);
        Assert.InRange(job.ModifiedAt, before, DateTimeOffset.Now);
    }

    [Fact]
    public void SaveWritesCamelCaseJsonFileNamedAfterId()
    {
        var (repository, fileSystem) = CreateRepository();
        var job = CreateJob();
        job.Script = "print('hi')";

        repository.Save(job);

        string file = Path.Combine(JobsDirectory, job.Id + ".json");
        Assert.True(fileSystem.FileExists(file));
        using var document = JsonDocument.Parse(fileSystem.ReadAllText(file));
        var properties = document.RootElement.EnumerateObject().Select(p => p.Name).ToList();
        Assert.Equal(
            new[] { "id", "name", "description", "script", "createdAt", "modifiedAt" },
            properties);
        Assert.Equal(job.Id.ToString(), document.RootElement.GetProperty("id").GetString());
        Assert.Equal("print('hi')", document.RootElement.GetProperty("script").GetString());
    }

    [Fact]
    public void SaveKeepsExistingIdCreatedAtAndUpdatesModifiedAt()
    {
        var (repository, _) = CreateRepository();
        var job = CreateJob();
        repository.Save(job);
        Guid id = job.Id;
        var createdAt = job.CreatedAt;

        job.Name = "Renamed job";
        repository.Save(job);

        Assert.Equal(id, job.Id);
        Assert.Equal(createdAt, job.CreatedAt);
        Assert.True(job.ModifiedAt > createdAt);
    }

    [Fact]
    public void LoadReturnsSavedJobWithAllValues()
    {
        var (repository, _) = CreateRepository();
        var job = new Job
        {
            Id = Guid.NewGuid(),
            Name = "Round trip",
            Description = "description",
            Script = "log('round trip')",
            CreatedAt = DateTimeOffset.Now.AddDays(-1),
            ModifiedAt = DateTimeOffset.Now
        };
        repository.Save(job);

        var loaded = repository.Load(job.Id);

        Assert.NotNull(loaded);
        Assert.Equal(job.Id, loaded.Id);
        Assert.Equal("Round trip", loaded.Name);
        Assert.Equal("description", loaded.Description);
        Assert.Equal("log('round trip')", loaded.Script);
        Assert.Equal(job.CreatedAt, loaded.CreatedAt);
        Assert.Equal(job.ModifiedAt, loaded.ModifiedAt);
        Assert.False(loaded.IsTemporary);
    }

    [Fact]
    public void LoadReturnsNullForUnknownJob()
    {
        var (repository, _) = CreateRepository();

        Assert.Null(repository.Load(Guid.NewGuid()));
    }

    [Fact]
    public void LoadAllReturnsAllPersistedJobs()
    {
        var (repository, _) = CreateRepository();
        var first = CreateJob("first");
        var second = CreateJob("second");
        repository.Save(first);
        repository.Save(second);

        var jobs = repository.LoadAll();

        Assert.Equal(2, jobs.Count);
        Assert.Contains(jobs, job => job.Name == "first");
        Assert.Contains(jobs, job => job.Name == "second");
    }

    [Fact]
    public void LoadAllSkipsMalformedJobFiles()
    {
        var (repository, fileSystem) = CreateRepository();
        var valid = CreateJob("valid");
        repository.Save(valid);
        fileSystem.WriteAllText(Path.Combine(JobsDirectory, Guid.NewGuid() + ".json"), "this is not JSON");

        var jobs = repository.LoadAll();

        var job = Assert.Single(jobs);
        Assert.Equal("valid", job.Name);
    }

    [Fact]
    public void LoadAllSkipsIncompleteJobFiles()
    {
        var (repository, fileSystem) = CreateRepository();
        fileSystem.CreateDirectory(JobsDirectory);
        fileSystem.WriteAllText(
            Path.Combine(JobsDirectory, Guid.NewGuid() + ".json"),
            @"{ ""name"": ""missing other fields"" }");

        Assert.Empty(repository.LoadAll());
    }

    [Fact]
    public void SaveLeavesNoTemporaryFileBehind()
    {
        var (repository, fileSystem) = CreateRepository();
        var job = CreateJob();

        repository.Save(job);

        var files = fileSystem.GetFiles(JobsDirectory, "*");
        var file = Assert.Single(files);
        Assert.EndsWith(job.Id + ".json", file);
    }

    [Fact]
    public void DeleteRemovesJobFile()
    {
        var (repository, fileSystem) = CreateRepository();
        var job = CreateJob();
        repository.Save(job);

        Assert.True(repository.Delete(job.Id));
        Assert.Null(repository.Load(job.Id));
        Assert.Empty(fileSystem.GetFiles(JobsDirectory, "*.json"));
    }

    [Fact]
    public void DeleteReturnsFalseForUnknownJob()
    {
        var (repository, _) = CreateRepository();

        Assert.False(repository.Delete(Guid.NewGuid()));
    }
}

/// <summary>
/// Concurrency and atomicity are properties of the real file system (rename
/// semantics), so these tests intentionally use FileSystemService with actual
/// disk access instead of the in-memory fake.
/// </summary>
public class JobRepositoryConcurrencyTests : IDisposable
{
    private readonly string tempDirectory = Path.Combine(
        Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());

    public void Dispose()
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    private JobRepository CreateRepository()
    {
        var platformDirectories = new PlatformDirectoryService(
            DesktopPlatform.MacOS, _ => null, tempDirectory);
        return new JobRepository(platformDirectories, new FileSystemService());
    }

    [Fact]
    public async Task ConcurrentSavesOfSameJobKeepFileComplete()
    {
        var repository = CreateRepository();
        var job = new Job { Name = "Test job", Script = "log('hello')" };

        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => Task.Run(() =>
        {
            job.Name = "name " + Guid.NewGuid();
            repository.Save(job);
        })));

        var loaded = repository.Load(job.Id);
        Assert.NotNull(loaded);
        Assert.Equal(job.Id, loaded.Id);
        string jobsDirectory = Path.Combine(
            tempDirectory, "Library", "Application Support", "Taskington", "jobs");
        var files = Directory.GetFiles(jobsDirectory);
        Assert.Single(files);
        Assert.EndsWith(".json", files[0]);
    }

    [Fact]
    public async Task ConcurrentSavesOfDifferentJobsAllPersist()
    {
        var repository = CreateRepository();
        var jobs = Enumerable.Range(0, 10)
            .Select(i => new Job { Name = "job " + i, Script = "log('hello')" })
            .ToList();

        await Task.WhenAll(jobs.Select(job => Task.Run(() => repository.Save(job))));

        var names = repository.LoadAll().Select(job => job.Name).ToList();
        Assert.Equal(10, names.Count);
        Assert.All(Enumerable.Range(0, 10), i => Assert.Contains("job " + i, names));
    }
}
