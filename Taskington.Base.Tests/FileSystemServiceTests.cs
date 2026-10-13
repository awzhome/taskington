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

    [Fact]
    public void ReadAllTextReturnsFileContent()
    {
        string file = Path.Combine(tempDirectory, "read.txt");
        Directory.CreateDirectory(tempDirectory);
        File.WriteAllText(file, "file content");

        Assert.Equal("file content", fileSystem.ReadAllText(file));
    }

    [Fact]
    public void WriteAllTextWritesAndOverwrites()
    {
        string file = Path.Combine(tempDirectory, "write.txt");
        Directory.CreateDirectory(tempDirectory);

        fileSystem.WriteAllText(file, "first");
        fileSystem.WriteAllText(file, "second");

        Assert.Equal("second", File.ReadAllText(file));
    }

    [Fact]
    public void DeleteFileRemovesExistingFile()
    {
        string file = Path.Combine(tempDirectory, "delete.txt");
        Directory.CreateDirectory(tempDirectory);
        File.WriteAllText(file, "content");

        fileSystem.DeleteFile(file);

        Assert.False(File.Exists(file));
    }

    [Fact]
    public void DeleteFileIsSafeForMissingFile()
    {
        string file = Path.Combine(tempDirectory, "missing.txt");
        Directory.CreateDirectory(tempDirectory);

        fileSystem.DeleteFile(file);

        Assert.False(File.Exists(file));
    }

    [Fact]
    public void MoveFileOverwritesDestinationWhenAllowed()
    {
        string source = Path.Combine(tempDirectory, "source.txt");
        string destination = Path.Combine(tempDirectory, "destination.txt");
        Directory.CreateDirectory(tempDirectory);
        File.WriteAllText(source, "source content");
        File.WriteAllText(destination, "destination content");

        fileSystem.MoveFile(source, destination, overwrite: true);

        Assert.Equal("source content", File.ReadAllText(destination));
        Assert.False(File.Exists(source));
    }

    [Fact]
    public void MoveFileThrowsWhenDestinationExistsAndOverwriteNotAllowed()
    {
        string source = Path.Combine(tempDirectory, "source.txt");
        string destination = Path.Combine(tempDirectory, "destination.txt");
        Directory.CreateDirectory(tempDirectory);
        File.WriteAllText(source, "source content");
        File.WriteAllText(destination, "destination content");

        Assert.Throws<IOException>(() => fileSystem.MoveFile(source, destination, overwrite: false));
    }

    [Fact]
    public void GetFilesReturnsMatchingFilesWithoutSubdirectories()
    {
        Directory.CreateDirectory(Path.Combine(tempDirectory, "sub"));
        File.WriteAllText(Path.Combine(tempDirectory, "a.json"), "{}");
        File.WriteAllText(Path.Combine(tempDirectory, "b.json"), "{}");
        File.WriteAllText(Path.Combine(tempDirectory, "c.txt"), "");
        File.WriteAllText(Path.Combine(tempDirectory, "sub", "d.json"), "{}");

        string[] files = fileSystem.GetFiles(tempDirectory, "*.json");

        Assert.Equal(2, files.Length);
        Assert.All(files, file => Assert.EndsWith(".json", file));
    }
}
