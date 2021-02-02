using System;
using System.IO;

namespace PPBackup.Base.Config
{
    public class YamlFileConfigurationProvider : IStreamReaderProvider, IStreamWriterProvider
    {
        public void ReadConfigurationStreams(Action<TextReader> configReader)
        {
            using var reader = new StreamReader(DetermineFileName());
            configReader(reader);
        }

        public void WriteConfigurationStreams(Action<TextWriter> configWriter)
        {
            using var writer = new StreamWriter(DetermineFileName());
            configWriter(writer);
        }

        private static string DetermineFileName()
        {
            string fileName = "ppbackup.yml";
            string userFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), fileName);
            string workDirFilePath = Path.Combine(Environment.CurrentDirectory, fileName);

            if (File.Exists(workDirFilePath))
            {
                return workDirFilePath;
            }
            else if (File.Exists(userFilePath))
            {
                return userFilePath;
            }

            throw new FileNotFoundException("No backup configuration file found.");
        }
    }
}
