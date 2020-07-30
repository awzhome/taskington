using PPBackup.Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace PPBackup.Base.Config
{
    public class YamlConfigurationReader
    {
        private readonly IYamlConfigurationProvider configurationProvider;

        public YamlConfigurationReader(IYamlConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public IEnumerable<BackupPlan> Read()
        {
            List<BackupPlan> plans = new List<BackupPlan>();

            using (var reader = configurationProvider.OpenConfiguration())
            {
                var yamlStream = new YamlStream();
                try
                {
                    yamlStream.Load(reader);

                    if (yamlStream.Documents[0].RootNode is YamlSequenceNode root)
                    {
                        foreach (var planEntry in root.Children.OfType<YamlMappingNode>())
                        {
                            var planType = (GetChildNode(planEntry.Children, "run") as YamlScalarNode)?.Value ?? "manually";
                            var plan = new BackupPlan(planType);

                            foreach (var planProperty in planEntry.Children)
                            {
                                if (planProperty.Key is YamlScalarNode scalarKey
                                    && planProperty.Value is YamlScalarNode scalarValue)
                                {
                                    if (scalarKey.Value == "run")
                                    {
                                        continue;
                                    }
                                    if (scalarKey.Value == "plan")
                                    {
                                        plan.Name = scalarValue.Value;
                                    }
                                    else
                                    {
                                        if (scalarKey.Value != null)
                                        {
                                            plan[scalarKey.Value] = (planProperty.Value as YamlScalarNode)?.Value;
                                        }
                                    }
                                }
                            }

                            if (GetChildNode(planEntry.Children, "steps") is YamlSequenceNode stepsEntries)
                            {
                                foreach (var stepEntry in stepsEntries.Children.OfType<YamlMappingNode>())
                                {
                                    if (stepEntry.Children.Any())
                                    {
                                        var firstYamlChild = stepEntry.Children.First();
                                        var stepType = (firstYamlChild.Key as YamlScalarNode)?.Value;

                                        if (stepType != null)
                                        {
                                            var step = new BackupStep(stepType);

                                            bool firstProperty = true;
                                            foreach (var stepProperty in stepEntry.Children)
                                            {
                                                if (stepProperty.Key is YamlScalarNode scalarKey
                                                    && stepProperty.Value is YamlScalarNode scalarValue)
                                                {
                                                    if (firstProperty)
                                                    {
                                                        step.DefaultProperty = scalarValue?.Value;
                                                    }
                                                    else
                                                    {
                                                        if (scalarKey.Value != null)
                                                        {
                                                            step[scalarKey.Value] = scalarValue?.Value;
                                                        }
                                                    }
                                                    firstProperty = false;
                                                }
                                            }

                                            plan.Steps.Add(step);
                                        }
                                        else
                                        {
                                            plan.Steps.Add(new InvalidBackupStep("Step has no type."));
                                        }
                                    }
                                }
                            }

                            plans.Add(plan);
                        }
                    }
                }
                catch (Exception ex) when (ex is SyntaxErrorException || ex is SemanticErrorException)
                {
                    throw new InvalidOperationException("Faulty backup plan configuration.", ex);
                }
            }

            return plans;
        }

        private YamlNode? GetChildNode(IDictionary<YamlNode, YamlNode> children, string key, YamlNode? defaultValue = default)
        {
            if (children.TryGetValue(new YamlScalarNode(key), out YamlNode value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
