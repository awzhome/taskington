using System.Threading;
using Taskington.Base.Log;

namespace Taskington.Base.SystemOperations;

class DryRunSystemOperations : ISystemOperations
{
    private readonly ILog log;

    public DryRunSystemOperations(ILog log)
    {
        this.log = log;
    }

    public void SyncDirectory(SyncDirection direction, string from, string to)
    {
        var directionOutput = direction switch
        {
            SyncDirection.FromTo => "->",
            SyncDirection.Both => "<->",
            _ => "?-?"
        };
        log.Debug(this, $"SYSOP: Sync dir '{from}' {directionOutput} '{to}");

        Thread.Sleep(500);
    }


    public void SyncFile(string fromDir, string toDir, string fileName)
    {
        log.Debug(this, $"SYSOP: Sync file '{fileName}' '{fromDir}' -> '{toDir}'");

        Thread.Sleep(500);
    }

    public Placeholders LoadSystemPlaceholders()
    {
        // No-op
        return new Placeholders();
    }
}
