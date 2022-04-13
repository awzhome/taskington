using System;
using System.IO;

namespace Taskington.Base.Config
{
    public class YamlFileConfigurationProvider : IStreamReaderProvider, IStreamWriterProvider
    {
        static string AppRoamingPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "taskington");
        static string UserFilePath => Path.Combine(AppRoamingPath, "taskington.yml");

        public void ReadConfigurationStreams(Action<TextReader> configReader)
        {
            var settingsFile = UserFilePath;
            if (File.Exists(settingsFile))
            {
                using var reader = new StreamReader(settingsFile);
                configReader(reader);
            }
            else
            {
                using var memoryStream = new MemoryStream();
                using var reader = new StreamReader(memoryStream);
                configReader(reader);
            }
        }

        public void WriteConfigurationStreams(Action<TextWriter> configWriter)
        {
            using var writer = new StreamWriter(UserFilePath);
            configWriter(writer);
        }
    }
}
