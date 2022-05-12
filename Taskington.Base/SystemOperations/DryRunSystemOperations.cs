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

            SystemOperationsMessages.SyncDirectory.Subscribe(SyncDirectory);
            SystemOperationsMessages.SyncFile.Subscribe(SyncFile);
            SystemOperationsMessages.LoadSystemPlaceholders.Subscribe(WindowsSystemOperations.LoadWindowsSystemPlaceholders);
        }

        public void SyncDirectory(SyncDirection syncDirection, string fromDir, string toDir)
        {
            var syncDirectionOutput = syncDirection switch
            {
                SyncDirection.FromTo => "->",
                SyncDirection.Both => "<->",
                _ => "?-?"
            };
            log.Debug(this, $"SYSOP: Sync dir '{fromDir}' {syncDirectionOutput} '{toDir}");

            Thread.Sleep(500);
        }

        public void SyncFile(string fromDir, string toDir, string file)
        {
            log.Debug(this, $"SYSOP: Sync file '{file}' '{fromDir}' -> '{toDir}'");

            Thread.Sleep(500);
        }
    }
}
