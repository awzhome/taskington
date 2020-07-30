using System;
using System.IO;

namespace PPBackup.Base.SystemOperations
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
        }

        public void SyncFile(string fromDir, string toDir, string file)
        {
            Console.WriteLine($"SYSOP: Sync file '{file}' '{fromDir}' -> '{toDir}'");
        }

        public void LoadSystemPlaceholders(Placeholders placeholders)
        {
            WindowsSystemOperations.LoadWindowsSystemPlaceholders(placeholders);
        }
    }
}
