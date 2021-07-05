using System;
using System.IO;
using System.Text;

namespace Taskington.Base.Config
{
    public class StringConfigurationProvider : IStreamReaderProvider, IStreamWriterProvider
    {
        private readonly StringBuilder contentBuffer = new StringBuilder();

        public StringConfigurationProvider()
        {
        }

        public StringConfigurationProvider(string yamlContent)
        {
            contentBuffer.Append(yamlContent);
        }

        public void ReadConfigurationStreams(Action<TextReader> configReader)
        {
            using var reader = new StringReader(Content);
            configReader(reader);
        }

        public void WriteConfigurationStreams(Action<TextWriter> configWriter)
        {
            contentBuffer.Clear();
            using var writer = new StringWriter(contentBuffer);
            configWriter(writer);
        }

        public string Content => contentBuffer.ToString();
    }
}
