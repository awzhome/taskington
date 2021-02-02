using PPBackup.Base.Model;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace PPBackup.Base.Config
{
    public class YamlConfigurationWriter
    {
        private readonly IStreamWriterProvider configWriterProvider;

        public YamlConfigurationWriter(IStreamWriterProvider configWriterProvider)
        {
            this.configWriterProvider = configWriterProvider;
        }

        public void Write(IEnumerable<BackupPlan> plans)
        {
            configWriterProvider.WriteConfigurationStreams(writer =>
            {
                var serializer = new Serializer();
                var yamlRoot = new YamlSequenceNode(
                    plans.Select(plan =>
                    {
                        var mappings = new List<KeyValuePair<YamlNode, YamlNode>>
                        {
                        StringMapping("plan", plan.Name),
                        StringMapping("run", plan.RunType)
                        };
                        mappings.AddRange(plan.Properties.Select(keyValue => StringMapping(keyValue.Key, keyValue.Value)));
                        mappings.Add(Mapping("steps", new YamlSequenceNode(
                            plan.Steps.Select(step => new YamlMappingNode(
                                new[] { StringMapping(step.StepType, step.DefaultProperty) }.Concat(
                                    step.Properties.Select(keyValue => StringMapping(keyValue.Key, keyValue.Value))
                                )
                            ))
                        )));

                        return new YamlMappingNode(mappings);
                    }));

                serializer.Serialize(writer, yamlRoot);
            });
        }

        private static KeyValuePair<YamlNode, YamlNode> Mapping(string key, YamlNode value) =>
            KeyValuePair.Create<YamlNode, YamlNode>(new YamlScalarNode(key), value);

        private static KeyValuePair<YamlNode, YamlNode> StringMapping(string key, string? value) =>
            KeyValuePair.Create<YamlNode, YamlNode>(new YamlScalarNode(key), new YamlScalarNode(value));
    }
}
