using System.Threading;
using Taskington.Base.Log;

namespace Taskington.Base.SystemOperations
{
    class DryRunSystemOperations
    {
        private readonly ILog log;

        public DryRunSystemOperations(ILog log)
        {
            this.log = log;

            SyncDirectoryMessage.Subscribe(SyncDirectory);
            SyncFileMessage.Subscribe(SyncFile);
            LoadSystemPlaceholdersMessage.Subscribe(WindowsSystemOperations.LoadWindowsSystemPlaceholders);
        }

        public void SyncDirectory(SyncDirectoryMessage message)
        {
            (SyncDirection syncDirection, string fromDir, string toDir) = message;
            var syncDirectionOutput = syncDirection switch
            {
                SyncDirection.FromTo => "->",
                SyncDirection.Both => "<->",
                _ => "?-?"
            };
            log.Debug(this, $"SYSOP: Sync dir '{fromDir}' {syncDirectionOutput} '{toDir}");

            Thread.Sleep(500);
        }

        public void SyncFile(SyncFileMessage message)
        {
            (string fromDir, string toDir, string file) = message;
            log.Debug(this, $"SYSOP: Sync file '{file}' '{fromDir}' -> '{toDir}'");

            Thread.Sleep(500);
        }
    }
}
