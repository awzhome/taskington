using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Taskington.Base.SystemOperations
{
    internal class WindowsSystemOperations
    {

        public WindowsSystemOperations()
        {
            SystemOperationsEvents.SyncDirectory.Subscribe((direction, from, to) => SyncDirectory(from, to));
            SystemOperationsEvents.SyncFile.Subscribe(SyncFile);
            SystemOperationsEvents.LoadSystemPlaceholders.Subscribe(LoadWindowsSystemPlaceholders);
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
                        var a when a.Contains(" ") => $"\"{arg}\"",
                        _ => arg
                    }))
                }
            };
            process.Start();
            process.StandardOutput.ReadToEnd();
        }

        private static void RunRoboCopy(params string[] arguments) =>
            RunProcess("robocopy", arguments);

        public static void SyncDirectory(string fromDir, string toDir) =>
            RunRoboCopy(fromDir, toDir, "/MIR", "/FFT", "/NFL", "/NJH", "/NJS" /*, "/L"*/);

        public void SyncFile(string fromDir, string toDir, string file) =>
            RunRoboCopy(fromDir, toDir, file, "/NFL", "/NJH", "/NJS" /*, "/L"*/);

        internal static Placeholders LoadWindowsSystemPlaceholders()
        {
            var placeholders = new Placeholders();
            var systemDrives = DriveInfo.GetDrives();
            foreach (var drive in systemDrives)
            {
                placeholders[$"drive:{drive.VolumeLabel}"] = drive.RootDirectory.FullName.TrimEnd(Path.DirectorySeparatorChar);
            }

            placeholders["AppDataRoaming"] = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            return placeholders;
        }
    }
}
