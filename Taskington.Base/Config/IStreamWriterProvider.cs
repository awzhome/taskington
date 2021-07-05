using System;
using System.IO;

namespace Taskington.Base.Config
{
    public interface IStreamWriterProvider
    {
        void WriteConfigurationStreams(Action<TextWriter> configWriter);
    }
}
