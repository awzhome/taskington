using System.IO;

namespace PPBackup.Base.Config
{
    public class YamlStringConfigurationProvider : IConfigurationProvider
    {
        private readonly string yamlContent;

        public YamlStringConfigurationProvider(string yamlContent)
        {
            this.yamlContent = yamlContent;
        }

        public TextReader OpenConfiguration()
        {
            return new StringReader(yamlContent);
        }
    }
}
