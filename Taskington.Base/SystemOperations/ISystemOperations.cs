namespace Taskington.Base.SystemOperations;

public interface ISystemOperations
{
    void SyncDirectory(SyncDirection direction, string from, string to);
    void SyncFile(string fromDir, string toDir, string fileName);
    Placeholders LoadSystemPlaceholders();
}