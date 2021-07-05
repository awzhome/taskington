using System;
using System.Collections.Generic;
using System.Linq;
using Taskington.Base.Plans;
using Taskington.Base.Steps;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Taskington.Base.Config
{
    public class YamlConfigurationReader
    {
        private readonly IStreamReaderProvider configurationProvider;

        public YamlConfigurationReader(IStreamReaderProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public IEnumerable<BackupPlan> Read()
        {
            List<BackupPlan> plans = new();

            configurationProvider.ReadConfigurationStreams(reader =>
            {
                var yamlStream = new YamlStream();
                try
                {
                    yamlStream.Load(reader);

                    if (yamlStream.Documents.Count > 0 && yamlStream.Documents[0].RootNode is YamlSequenceNode root)
                    {
                        foreach (var planEntry in root.Children.OfType<YamlMappingNode>())
                        {
                            List<BackupStep> steps = new();
                            var planType = (GetChildNode(planEntry.Children, "on") as YamlScalarNode)?.Value ?? BackupPlan.OnSelectionRunType;
                            var plan = new BackupPlan(planType)
                            {
                                Steps = steps
                            };

                            foreach (var planProperty in planEntry.Children)
                            {
                                if (planProperty.Key is YamlScalarNode scalarKey
                                    && planProperty.Value is YamlScalarNode scalarValue)
                                {
                                    if (scalarKey.Value == "on")
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

                                            steps.Add(step);
                                        }
                                        else
                                        {
                                            steps.Add(new InvalidBackupStep("Step has no type."));
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
            });

            return plans;
        }

        private static YamlNode? GetChildNode(IDictionary<YamlNode, YamlNode> children, string key, YamlNode? defaultValue = default)
        {
            if (children.TryGetValue(new YamlScalarNode(key), out YamlNode? value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
