using System.IO;

namespace PPBackup.Base.Config
{
    public interface IConfigurationStreamProvider
    {
        TextReader CreateConfigurationReader();
        TextWriter CreateConfigurationWriter();
    }
}
