namespace Taskington.Base.SystemOperations;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

internal class MacOsSystemOperations : ISystemOperations
{
    private static void RunProcess(string fileName, params string[] arguments)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                UseShellExecute = false,
                FileName = fileName,
                Arguments = string.Join(" ", arguments.Select(arg => arg switch
                {
                    var a when a.Contains(' ') => $"\"{arg}\"",
                    _ => arg
                }))
            }
        };
        process.Start();
        process.StandardOutput.ReadToEnd();
    }

    private static void RunRSync(params string[] arguments) =>
        RunProcess("rsync", arguments);

    public void SyncDirectory(SyncDirection direction, string from, string to)
    {
        if (!Directory.Exists(to))
        {
            Directory.CreateDirectory(to);
        }
        RunRSync("-r", "--delete", "-t", $"{from}/", to);
    }

    public void SyncFile(string fromDir, string toDir, string fileName)
    {
        RunRSync("-t", Path.Combine(fromDir, fileName), toDir);
    }

    public Placeholders LoadSystemPlaceholders()
    {
        var placeholders = new Placeholders();
        placeholders["AppDataRoaming"] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        return placeholders;
    }
}
