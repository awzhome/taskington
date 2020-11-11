using System;
using System.IO;

namespace PPBackup.Base.Config
{
    public class YamlFileConfigurationProvider : IConfigurationStreamProvider
    {
        public TextReader CreateConfigurationReader()
        {
            return new StreamReader(DetermineFileName());
        }

        public TextWriter CreateConfigurationWriter()
        {
            return new StreamWriter(DetermineFileName());
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
