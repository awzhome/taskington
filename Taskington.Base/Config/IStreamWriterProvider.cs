using System;
using System.IO;

namespace PPBackup.Base.Config
{
    public interface IStreamWriterProvider
    {
        void WriteConfigurationStreams(Action<TextWriter> configWriter);
    }
}
