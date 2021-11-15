using System;
using System.Collections.Generic;
using System.IO;
using Taskington.Base.Events;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Config
{
    public abstract class WatchingFileReaderProvider : IStreamReaderProvider, IDisposable
    {
        private FileSystemWatcher? configFileWatcher;
        private readonly IEventBus eventBus;

        protected static string AppRoamingPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "taskington");

        public WatchingFileReaderProvider(IEventBus eventBus)
        {
            this.eventBus = eventBus;

            configFileWatcher = new FileSystemWatcher(GetConfigDirectory(), WatchedFilesFilter)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.DirectoryName | NotifyFilters.FileName
            };
            configFileWatcher.Created += OnFileChanged;
            configFileWatcher.Changed += OnFileChanged;
            configFileWatcher.Renamed += (sender, e) => eventBus.Push(new ConfigurationChanged());
            configFileWatcher.Deleted += (sender, e) => eventBus.Push(new ConfigurationChanged());
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

                eventBus.Push(new ConfigurationChanged());
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
