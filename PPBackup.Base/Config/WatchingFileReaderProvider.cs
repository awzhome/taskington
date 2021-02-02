using System;
using System.Collections.Generic;
using System.IO;

namespace PPBackup.Base.Config
{
    public abstract class WatchingFileReaderProvider : IStreamReaderProvider, IDisposable
    {
        private FileSystemWatcher? configFileWatcher;

        protected static string AppRoamingPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ppbackup");

        public WatchingFileReaderProvider(ApplicationEvents events)
        {
            configFileWatcher = new FileSystemWatcher(GetConfigDirectory(), WatchedFilesFilter)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size
            };
            configFileWatcher.Created += (sender, e) => events.ConfigurationChange();
            configFileWatcher.Changed += (sender, e) => events.ConfigurationChange();
            configFileWatcher.Deleted += (sender, e) => events.ConfigurationChange();
            configFileWatcher.EnableRaisingEvents = true;
        }

        ~WatchingFileReaderProvider()
        {
            DisposeFileWatcher();
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
