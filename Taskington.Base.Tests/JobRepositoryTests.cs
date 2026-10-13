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

public class JobRepositoryTests : IDisposable
{
    private readonly string tempDirectory = Path.Combine(
        Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());
    private readonly JobRepository repository;

    public JobRepositoryTests()
    {
        var platformDirectories = new PlatformDirectoryService(
            DesktopPlatform.MacOS, _ => null, tempDirectory);
        repository = new JobRepository(platformDirectories, new FileSystemService());
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    private string JobsDirectory => Path.Combine(
        tempDirectory, "Library", "Application Support", "Taskington", "jobs");

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
        var job = CreateJob();
        job.Script = "print('hi')";

        repository.Save(job);

        string file = Path.Combine(JobsDirectory, job.Id + ".json");
        Assert.True(File.Exists(file));
        using var document = JsonDocument.Parse(File.ReadAllText(file));
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
        Assert.Equal(job.Id, loaded!.Id);
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
        Assert.Null(repository.Load(Guid.NewGuid()));
    }

    [Fact]
    public void LoadAllReturnsAllPersistedJobs()
    {
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
        var valid = CreateJob("valid");
        repository.Save(valid);
        string malformedPath = Path.Combine(JobsDirectory, Guid.NewGuid() + ".json");
        File.WriteAllText(malformedPath, "this is not JSON");

        var jobs = repository.LoadAll();

        var job = Assert.Single(jobs);
        Assert.Equal("valid", job.Name);
    }

    [Fact]
    public void LoadAllSkipsIncompleteJobFiles()
    {
        Directory.CreateDirectory(JobsDirectory);
        string incompletePath = Path.Combine(JobsDirectory, Guid.NewGuid() + ".json");
        File.WriteAllText(incompletePath, @"{ ""name"": ""missing other fields"" }");

        Assert.Empty(repository.LoadAll());
    }

    [Fact]
    public void SaveLeavesNoTemporaryFileBehind()
    {
        var job = CreateJob();
        repository.Save(job);

        var files = Directory.GetFiles(JobsDirectory);
        var file = Assert.Single(files);
        Assert.EndsWith(job.Id + ".json", file);
    }

    [Fact]
    public void DeleteRemovesJobFile()
    {
        var job = CreateJob();
        repository.Save(job);

        Assert.True(repository.Delete(job.Id));
        Assert.Null(repository.Load(job.Id));
        Assert.Empty(Directory.GetFiles(JobsDirectory));
    }

    [Fact]
    public void DeleteReturnsFalseForUnknownJob()
    {
        Assert.False(repository.Delete(Guid.NewGuid()));
    }

    [Fact]
    public async Task ConcurrentSavesOfSameJobKeepFileComplete()
    {
        var job = CreateJob();

        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => Task.Run(() =>
        {
            job.Name = "name " + Guid.NewGuid();
            repository.Save(job);
        })));

        var loaded = repository.Load(job.Id);
        Assert.NotNull(loaded);
        Assert.Equal(job.Id, loaded!.Id);
        var files = Directory.GetFiles(JobsDirectory);
        Assert.Single(files);
        Assert.True(files[0].EndsWith(".json"));
    }

    [Fact]
    public async Task ConcurrentSavesOfDifferentJobsAllPersist()
    {
        var jobs = Enumerable.Range(0, 10)
            .Select(i => CreateJob("job " + i))
            .ToList();

        await Task.WhenAll(jobs.Select(job => Task.Run(() => repository.Save(job))));

        var names = repository.LoadAll().Select(job => job.Name).ToList();
        Assert.Equal(10, names.Count);
        Assert.All(Enumerable.Range(0, 10), i => Assert.Contains("job " + i, names));
    }
}
