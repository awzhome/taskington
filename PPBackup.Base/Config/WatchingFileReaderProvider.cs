using System;
using System.Collections.Generic;
using System.IO;

namespace PPBackup.Base.Config
{
    public abstract class WatchingFileReaderProvider : IStreamReaderProvider, IDisposable
    {
        private FileSystemWatcher? configFileWatcher;
        private readonly ApplicationEvents events;

        protected static string AppRoamingPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ppbackup");

        public WatchingFileReaderProvider(ApplicationEvents events)
        {
            this.events = events;

            configFileWatcher = new FileSystemWatcher(GetConfigDirectory(), WatchedFilesFilter)
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
            configFileWatcher.Created += (sender, e) => events.ConfigurationChange();
            configFileWatcher.Changed += OnFileChanged;
            configFileWatcher.Deleted += (sender, e) => events.ConfigurationChange();
            configFileWatcher.EnableRaisingEvents = true;
        }

        ~WatchingFileReaderProvider()
        {
            DisposeFileWatcher();
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                // This is needed, because we might receive Changed events while file is still open and written
                FileInfo file = new(e.FullPath);
                using (var fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fileStream.Close();
                }

                events.ConfigurationChange();
            }
            catch (IOException) { }
        }

        public void ReadConfigurationStreams(Action<TextReader> configReader)
        {
            foreach (var file in GetFileNames())
            {
                using var reader = new StreamReader(file);
                configReader(reader);
            }
        }

        protected abstract IEnumerable<string> GetFileNames();

        protected abstract string GetConfigDirectory();

        protected virtual string WatchedFilesFilter => "*";

        public void Dispose()
        {
            DisposeFileWatcher();
            GC.SuppressFinalize(this);
        }

        private void DisposeFileWatcher()
        {
            configFileWatcher?.Dispose();
            configFileWatcher = null;
        }
    }
}
