using System;
using System.Threading;
using Taskington.Base.Events;
using Taskington.Base.Log;
using Taskington.Base.TinyBus;

namespace Taskington.Base.SystemOperations
{
    class DryRunSystemOperations
    {
        private readonly IEventBus eventBus;
        private readonly ILog log;

        public DryRunSystemOperations(IEventBus eventBus, ILog log)
        {
            this.eventBus = eventBus;
            this.log = log;
            eventBus
                .Subscribe<SyncDirectory>(SyncDirectory)
                .Subscribe<SyncFile>(SyncFile)
                .Subscribe<LoadSystemPlaceholders, Placeholders>(LoadSystemPlaceholders);
        }

        public void SyncDirectory(SyncDirectory e)
        {
            (SyncDirection syncDirection, string fromDir, string toDir) = e;

            var syncDirectionOutput = syncDirection switch
            {
                SyncDirection.FromTo => "->",
                SyncDirection.Both => "<->",
                _ => "?-?"
            };
            log.Debug(this, $"SYSOP: Sync dir '{fromDir}' {syncDirectionOutput} '{toDir}");

            Thread.Sleep(500);
        }

        public void SyncFile(SyncFile e)
        {
            (string fromDir, string toDir, string file) = e;

            log.Debug(this, $"SYSOP: Sync file '{file}' '{fromDir}' -> '{toDir}'");

            Thread.Sleep(500);
        }

        public Placeholders LoadSystemPlaceholders(LoadSystemPlaceholders e) => WindowsSystemOperations.LoadWindowsSystemPlaceholders(e);
    }
}
