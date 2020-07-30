using System.IO;

namespace PPBackup.Base.Config
{
    public interface IYamlConfigurationProvider
    {
        TextReader OpenConfiguration();
    }
}
