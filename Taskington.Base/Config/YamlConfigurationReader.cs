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

        public Configuration Read()
        {
            var configValues = new List<(string, string?)>();
            var plans = new List<Plan>();

            configurationProvider.ReadConfigurationStreams(reader =>
            {
                var yamlStream = new YamlStream();
                try
                {
                    yamlStream.Load(reader);

                    if (yamlStream.Documents.Count > 0 && yamlStream.Documents[0].RootNode is YamlMappingNode root)
                    {
                        if (GetChildNode(root.Children, "config") is YamlMappingNode configEntries)
                        {
                            foreach (var entry in configEntries.Children)
                            {
                                if (entry.Key is YamlScalarNode configScalarKey
                                        && entry.Value is YamlScalarNode configScalarValue)
                                {
                                    if (configScalarKey.Value != null)
                                    {
                                        configValues.Add((configScalarKey.Value, configScalarValue.Value));
                                    }
                                }
                            }
                        }

                        if (GetChildNode(root.Children, "plans") is YamlSequenceNode planEntries)
                        {
                            foreach (var planEntry in planEntries.OfType<YamlMappingNode>())
                            {
                                List<PlanStep> steps = new();
                                var planType = (GetChildNode(planEntry.Children, "on") as YamlScalarNode)?.Value ?? Plan.OnSelectionRunType;
                                var plan = new Plan(planType)
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
                                                var step = new PlanStep(stepType);

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
                                                steps.Add(new InvalidPlanStep("Step has no type."));
                                            }
                                        }
                                    }
                                }

                                plans.Add(plan);
                            }
                        }
                    }
                }
                catch (Exception ex) when (ex is SyntaxErrorException || ex is SemanticErrorException)
                {
                    throw new InvalidOperationException("Faulty plan configuration.", ex);
                }
            });

            return new Configuration(configValues, plans);
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
