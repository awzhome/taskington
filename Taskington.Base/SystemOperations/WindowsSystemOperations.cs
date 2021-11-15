using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Taskington.Base.Events;
using Taskington.Base.TinyBus;

namespace Taskington.Base.SystemOperations
{
    internal class WindowsSystemOperations
    {
        private readonly IEventBus eventBus;

        public WindowsSystemOperations(IEventBus eventBus)
        {
            this.eventBus = eventBus;

            eventBus
                .Subscribe<SyncDirectory>(SyncDirectory)
                .Subscribe<SyncFile>(SyncFile)
                .Subscribe<LoadSystemPlaceholders, Placeholders>(LoadWindowsSystemPlaceholders);
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

        private void RunRoboCopy(params string[] arguments) =>
            RunProcess("robocopy", arguments);

        public void SyncDirectory(SyncDirectory e) =>
            RunRoboCopy(e.FromDir, e.ToDir, "/MIR", "/FFT", "/NFL", "/NJH", "/NJS" /*, "/L"*/);

        public void SyncFile(SyncFile e) =>
            RunRoboCopy(e.FromDir, e.ToDir, e.File, "/NFL", "/NJH", "/NJS" /*, "/L"*/);

        internal static Placeholders LoadWindowsSystemPlaceholders(LoadSystemPlaceholders e)
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
