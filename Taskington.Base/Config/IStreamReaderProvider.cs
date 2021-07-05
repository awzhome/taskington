using System;
using System.IO;

namespace Taskington.Base.Config
{
    public interface IStreamReaderProvider
    {
        void ReadConfigurationStreams(Action<TextReader> configReader);
    }
}
