using System;
using System.IO;
using System.Threading;

namespace Taskington.Base.SystemOperations
{
    class DryRunSystemOperations : ISystemOperations
    {
        public void SyncDirectory(SyncDirection syncDirection, string fromDir, string toDir)
        {
            var syncDirectionOutput = syncDirection switch
            {
                SyncDirection.FromTo => "->",
                SyncDirection.Both => "<->",
                _ => "?-?"
            };
            Console.WriteLine($"SYSOP: Sync dir '{fromDir}' {syncDirectionOutput} '{toDir}");

            Thread.Sleep(500);
        }

        public void SyncFile(string fromDir, string toDir, string file)
        {
            Console.WriteLine($"SYSOP: Sync file '{file}' '{fromDir}' -> '{toDir}'");

            Thread.Sleep(500);
        }

        public void LoadSystemPlaceholders(Placeholders placeholders)
        {
            WindowsSystemOperations.LoadWindowsSystemPlaceholders(placeholders);
        }
    }
}
