using System.IO;

namespace PPBackup.Base.Config
{
    public interface IConfigurationProvider
    {
        TextReader OpenConfiguration();
    }
}
