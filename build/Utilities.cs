using System.Collections.Generic;
using System.IO;
using static Nuke.Common.EnvironmentInfo;

static class Utilities
{
    public static IEnumerable<string> FindFiles(string pattern) =>
        Directory.EnumerateFiles(WorkingDirectory, pattern, new EnumerationOptions() { RecurseSubdirectories = true });
}
