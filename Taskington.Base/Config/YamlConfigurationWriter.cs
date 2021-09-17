using System.Collections.Generic;
using System.Linq;
using Taskington.Base.Plans;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Taskington.Base.Config
{
    public class YamlConfigurationWriter
    {
        private readonly IStreamWriterProvider configWriterProvider;

        public YamlConfigurationWriter(IStreamWriterProvider configWriterProvider)
        {
            this.configWriterProvider = configWriterProvider;
        }

        public void Write(Configuration configuration)
        {
            configWriterProvider.WriteConfigurationStreams(writer =>
            {
                var yamlRoot = new YamlMappingNode();
                yamlRoot.Add("config", CreateConfigRoot(configuration.ConfigValues));
                yamlRoot.Add("plans", CreatePlansRoot(configuration.Plans));

                var serializer = new Serializer();
                serializer.Serialize(writer, yamlRoot);
            });
        }

        private static YamlMappingNode CreateConfigRoot(IEnumerable<(string Key, string? Value)> configValues) =>
            new YamlMappingNode(
                configValues.Select(keyValue => StringMapping(keyValue.Key, keyValue.Value)));

        private static YamlSequenceNode CreatePlansRoot(IEnumerable<Plan> plans) =>
            new YamlSequenceNode(
                plans.Select(plan =>
                {
                    var mappings = new List<KeyValuePair<YamlNode, YamlNode>>
                    {
                    StringMapping("plan", plan.Name),
                    };
                    mappings.Add(StringMapping("on", plan.RunType));
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

        private static KeyValuePair<YamlNode, YamlNode> Mapping(string key, YamlNode value) =>
            KeyValuePair.Create<YamlNode, YamlNode>(new YamlScalarNode(key), value);

        private static KeyValuePair<YamlNode, YamlNode> StringMapping(string key, string? value) =>
            KeyValuePair.Create<YamlNode, YamlNode>(new YamlScalarNode(key), new YamlScalarNode(value));
    }
}
