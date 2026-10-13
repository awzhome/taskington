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

    /// <summary>
    /// Reads the entire content of a text file.
    /// </summary>
    string ReadAllText(string path);

    /// <summary>
    /// Writes the entire content of a text file, overwriting an existing file.
    /// </summary>
    void WriteAllText(string path, string contents);

    /// <summary>
    /// Deletes the file if it exists; safe to call on a missing file.
    /// </summary>
    void DeleteFile(string path);

    /// <summary>
    /// Moves a file, optionally overwriting an existing destination file.
    /// </summary>
    void MoveFile(string sourcePath, string destinationPath, bool overwrite);

    /// <summary>
    /// Paths of the files in the given directory matching the search pattern
    /// (e.g. "*.json"), without subdirectories.
    /// </summary>
    string[] GetFiles(string directoryPath, string searchPattern);
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

    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    public void WriteAllText(string path, string contents)
    {
        File.WriteAllText(path, contents);
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
    }

    public void MoveFile(string sourcePath, string destinationPath, bool overwrite)
    {
        File.Move(sourcePath, destinationPath, overwrite);
    }

    public string[] GetFiles(string directoryPath, string searchPattern)
    {
        return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
    }
}
