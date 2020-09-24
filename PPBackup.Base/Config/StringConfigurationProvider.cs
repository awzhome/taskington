using System.IO;
using System.Text;

namespace PPBackup.Base.Config
{
    public class StringConfigurationProvider : IConfigurationStreamProvider
    {
        private readonly StringBuilder contentBuffer = new StringBuilder();

        public StringConfigurationProvider()
        {
        }

        public StringConfigurationProvider(string yamlContent)
        {
            contentBuffer.Append(yamlContent);
        }

        public TextReader CreateConfigurationReader()
        {
            return new StringReader(Content);
        }

        public TextWriter CreateConfigurationWriter()
        {
            contentBuffer.Clear();
            return new StringWriter(contentBuffer);
        }

        public string Content => contentBuffer.ToString();
    }
}
