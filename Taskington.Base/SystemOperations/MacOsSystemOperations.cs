namespace Taskington.Base.SystemOperations;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

internal class MacOsSystemOperations
{
    public MacOsSystemOperations()
    {
        SystemOperationsMessages.SyncDirectory.Subscribe((direction, from, to) => SyncDirectory(from, to));
        SystemOperationsMessages.SyncFile.Subscribe(SyncFile);
        SystemOperationsMessages.LoadSystemPlaceholders.Subscribe(LoadMacOsSystemPlaceholders);
    }

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

    public static void SyncDirectory(string fromDir, string toDir)
    {
        if (!Directory.Exists(toDir))
        {
            Directory.CreateDirectory(toDir);
        }
        RunRSync("-r", "--delete", $"{fromDir}/", toDir);
    }

    public void SyncFile(string fromDir, string toDir, string file) =>
        RunRSync(Path.Combine(fromDir, file), toDir);

    internal static Placeholders LoadMacOsSystemPlaceholders()
    {
        var placeholders = new Placeholders();
        placeholders["AppDataRoaming"] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        return placeholders;
    }
}
