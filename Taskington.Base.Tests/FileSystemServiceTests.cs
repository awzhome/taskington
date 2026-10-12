using System;
using System.IO;
using Taskington.Base.FileSystem;
using Xunit;

namespace Taskington.Base.Tests;

public class FileSystemServiceTests : IDisposable
{
    private readonly string tempDirectory = Path.Combine(
        Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());
    private readonly FileSystemService fileSystem = new();

    public void Dispose()
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    [Fact]
    public void CreateDirectoryCreatesMissingNestedDirectory()
    {
        string directory = Path.Combine(tempDirectory, "a", "b");

        fileSystem.CreateDirectory(directory);

        Assert.True(Directory.Exists(directory));
    }

    [Fact]
    public void CreateDirectoryIsSafeToCallRepeatedly()
    {
        string directory = Path.Combine(tempDirectory, "dir");

        fileSystem.CreateDirectory(directory);
        fileSystem.CreateDirectory(directory);

        Assert.True(Directory.Exists(directory));
    }

    [Fact]
    public void FileExistsReturnsTrueForExistingFile()
    {
        string file = Path.Combine(tempDirectory, "existing.txt");
        Directory.CreateDirectory(tempDirectory);
        File.WriteAllText(file, "content");

        Assert.True(fileSystem.FileExists(file));
    }

    [Fact]
    public void FileExistsReturnsFalseForMissingFile()
    {
        string file = Path.Combine(tempDirectory, "missing.txt");

        Assert.False(fileSystem.FileExists(file));
    }

    [Fact]
    public void CreateFileCreatesNewEmptyFile()
    {
        string file = Path.Combine(tempDirectory, "created.txt");
        Directory.CreateDirectory(tempDirectory);

        fileSystem.CreateFile(file);

        Assert.True(File.Exists(file));
        Assert.Empty(File.ReadAllText(file));
    }

    [Fact]
    public void CreateFileOverwritesExistingFile()
    {
        string file = Path.Combine(tempDirectory, "overwritten.txt");
        Directory.CreateDirectory(tempDirectory);
        File.WriteAllText(file, "old content");

        fileSystem.CreateFile(file);

        Assert.Empty(File.ReadAllText(file));
    }
}
