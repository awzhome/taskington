using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Taskington.Base.FileSystem;

namespace Taskington.Base.Tests;

/// <summary>
/// In-memory IFileSystemService for tests that exercise service logic without
/// disk access; behaves like the System.IO implementation for the operations the
/// services rely on (parent directory must exist for writes, GetFiles only
/// searches the given directory, MoveFile removes the source). Not thread-safe.
/// </summary>
internal sealed class InMemoryFileSystem : IFileSystemService
{
    private readonly Dictionary<string, string> files = new(StringComparer.Ordinal);
    private readonly HashSet<string> directories = new(StringComparer.Ordinal);

    public void CreateDirectory(string path)
    {
        AddDirectoryWithAncestors(path);
    }

    public bool FileExists(string path)
    {
        return files.ContainsKey(path);
    }

    public void CreateFile(string path)
    {
        EnsureParentDirectoryExists(path);
        files[path] = string.Empty;
    }

    public string ReadAllText(string path)
    {
        return files.TryGetValue(path, out string? contents)
            ? contents
            : throw new FileNotFoundException("File not found.", path);
    }

    public void WriteAllText(string path, string contents)
    {
        EnsureParentDirectoryExists(path);
        files[path] = contents;
    }

    public void DeleteFile(string path)
    {
        files.Remove(path);
    }

    public void MoveFile(string sourcePath, string destinationPath, bool overwrite)
    {
        if (!files.TryGetValue(sourcePath, out string? contents))
        {
            throw new FileNotFoundException("File not found.", sourcePath);
        }

        if (files.ContainsKey(destinationPath) && !overwrite)
        {
            throw new IOException("The destination file already exists.");
        }

        EnsureParentDirectoryExists(destinationPath);
        files[destinationPath] = contents;
        files.Remove(sourcePath);
    }

    public string[] GetFiles(string directoryPath, string searchPattern)
    {
        if (!directories.Contains(directoryPath))
        {
            throw new DirectoryNotFoundException("Directory not found: " + directoryPath);
        }

        var matcher = BuildPatternMatcher(searchPattern);
        return files.Keys
            .Where(path => Path.GetDirectoryName(path) == directoryPath)
            .Where(path => matcher.IsMatch(Path.GetFileName(path)))
            .ToArray();
    }

    private static Regex BuildPatternMatcher(string searchPattern)
    {
        string pattern = Regex.Escape(searchPattern).Replace(@"\*", ".*");
        return new Regex("^" + pattern + "$", RegexOptions.IgnoreCase);
    }

    private void AddDirectoryWithAncestors(string path)
    {
        string? current = path;
        while (current is not null && !directories.Contains(current))
        {
            directories.Add(current);
            current = Path.GetDirectoryName(current);
        }
    }

    private void EnsureParentDirectoryExists(string path)
    {
        string? parentDirectory = Path.GetDirectoryName(path);
        if (parentDirectory is null || !directories.Contains(parentDirectory))
        {
            throw new DirectoryNotFoundException("Directory not found: " + parentDirectory);
        }
    }
}
