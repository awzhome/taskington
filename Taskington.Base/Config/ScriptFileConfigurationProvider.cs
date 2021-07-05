using System;
using System.Collections.Generic;
using System.IO;

namespace Taskington.Base.Config
{
    public class ScriptFileConfigurationProvider : WatchingFileReaderProvider
    {
        public ScriptFileConfigurationProvider(ApplicationEvents events) : base(events)
        {
        }

        protected override IEnumerable<string> GetFileNames()
        {
            return Directory.GetFiles(GetConfigDirectory(), "*.ppbackup");
        }

        protected override string GetConfigDirectory()
        {
            var localConfig = Path.Combine(Environment.CurrentDirectory, "plans");
            return Directory.Exists(localConfig) ? localConfig : AppRoamingPath;
        }

        protected override string WatchedFilesFilter => "*.ppbackup";
    }
}
