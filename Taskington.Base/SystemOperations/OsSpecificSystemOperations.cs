namespace Taskington.Base.SystemOperations;

using System.Runtime.InteropServices;
using Taskington.Base.Log;


public static class OsSpecificSystemOperations
{
    public static ISystemOperations Create(ILog log)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsSystemOperations();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new MacOsSystemOperations();
        }

        return new DryRunSystemOperations(log);
    }
}