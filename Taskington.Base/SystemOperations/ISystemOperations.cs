namespace Taskington.Base.SystemOperations
{
    public interface ISystemOperations
    {
        void SyncDirectory(SyncDirection syncDirection, string fromDir, string toDir);

        void SyncFile(string fromDir, string toDir, string file);

        void LoadSystemPlaceholders(Placeholders placeholders);
    }
}
