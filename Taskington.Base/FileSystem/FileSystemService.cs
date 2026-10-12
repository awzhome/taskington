using System.IO;

namespace Taskington.Base.FileSystem;

/// <summary>
/// Basic file system operations, abstracted so services depending on them can be
/// unit tested without touching the real file system (spec chapter 7).
/// </summary>
public interface IFileSystemService
{
    /// <summary>
    /// Creates the directory if it does not exist yet; safe to call repeatedly.
    /// </summary>
    void CreateDirectory(string path);

    /// <summary>
    /// True if a file exists at the given path.
    /// </summary>
    bool FileExists(string path);

    /// <summary>
    /// Creates a new empty file, overwriting an existing file.
    /// </summary>
    void CreateFile(string path);
}

/// <summary>
/// File system access via System.IO.
/// </summary>
public sealed class FileSystemService : IFileSystemService
{
    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public void CreateFile(string path)
    {
        using (File.Create(path))
        {
        }
    }
}
