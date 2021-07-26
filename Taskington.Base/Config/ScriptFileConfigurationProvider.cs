using System;
using System.Collections.Generic;
using System.IO;
using Taskington.Base.Events;

namespace Taskington.Base.Config
{
    public class ScriptFileConfigurationProvider : WatchingFileReaderProvider
    {
        public ScriptFileConfigurationProvider(ApplicationEvents events) : base(events)
        {
        }

        protected override IEnumerable<string> GetFileNames()
        {
            return Directory.GetFiles(GetConfigDirectory(), "*.taskington");
        }

        protected override string GetConfigDirectory()
        {
            var localConfig = Path.Combine(Environment.CurrentDirectory, "plans");
            return Directory.Exists(localConfig) ? localConfig : AppRoamingPath;
        }

        protected override string WatchedFilesFilter => "*.taskington";
    }
}
