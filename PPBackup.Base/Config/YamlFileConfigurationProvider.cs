using System;
using System.IO;

namespace PPBackup.Base.Config
{
    public class YamlFileConfigurationProvider : IConfigurationProvider
    {
        public TextReader OpenConfiguration()
        {
            return new StreamReader(DetermineFileName());
        }

        private string DetermineFileName()
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
