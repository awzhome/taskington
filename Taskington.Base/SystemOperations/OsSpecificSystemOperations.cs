namespace Taskington.Base.SystemOperations;

using System.Runtime.InteropServices;

public static class OsSpecificSystemOperations
{
    public static object? Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WindowsSystemOperations();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new MacOsSystemOperations();
        }

        return null;
    }
}