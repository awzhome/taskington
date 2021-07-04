using System;
using System.IO;

namespace PPBackup.Base.Config
{
    public interface IStreamReaderProvider
    {
        void ReadConfigurationStreams(Action<TextReader> configReader);
    }
}
