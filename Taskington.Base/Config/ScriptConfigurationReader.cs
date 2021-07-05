using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Taskington.Base.Plans;
using Taskington.Base.Steps;

namespace Taskington.Base.Config
{
    public class ScriptConfigurationReader
    {
        private readonly IStreamReaderProvider configurationProvider;

        public ScriptConfigurationReader(IStreamReaderProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public IEnumerable<Plan> Read()
        {
            List<Plan> plans = new List<Plan>();

            configurationProvider.ReadConfigurationStreams(reader =>
            {
                List<PlanStep> steps = new();
                Plan plan = new(Plan.OnSelectionRunType)
                {
                    Steps = steps
                };

                bool beginOfFile = true;
                bool isFirstCollection = true;
                PlanStep? lastStep = null;
                foreach ((var key, var value, var newCollection) in ParseStream(reader))
                {
                    if (beginOfFile && !newCollection)
                    {
                        throw new InvalidOperationException("Faulty plan configuration.");
                    }

                    if (!beginOfFile && newCollection)
                    {
                        isFirstCollection = false;
                    }

                    if (isFirstCollection)
                    {
                        if (newCollection)
                        {
                            if (key == "plan")
                            {
                                plan.Name = value;
                            }
                            else
                            {
                                throw new InvalidOperationException("Faulty plan configuration.");
                            }
                        }
                        else
                        {
                            if (key == "on")
                            {
                                plan.RunType = value;
                            }
                            else
                            {
                                plan[key] = value;
                            }
                        }
                    }
                    else
                    {
                        if (newCollection)
                        {
                            lastStep = new PlanStep(key)
                            {
                                DefaultProperty = value
                            };
                            steps.Add(lastStep);
                        }
                        else if (lastStep != null)
                        {
                            lastStep[key] = value;
                        }
                    }

                    beginOfFile = false;
                }

                plans.Add(plan);
            });

            return plans;
        }

        private static readonly Regex linePattern = new Regex(
            @"(?<spaces>\s*)(?<key>\S+)\s+((?<value>((\$\{.*\})|[^\s""'])+)|""(?<value>[^""]*)""|'(?<value>[^']*)')",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private static IEnumerable<(string Key, string Value, bool NewCollection)> ParseStream(TextReader reader)
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (Match match in linePattern.Matches(line))
                {
                    yield return (
                        match.Groups["key"].Value,
                        match.Groups["value"].Value,
                        string.IsNullOrEmpty(match.Groups["spaces"].Value));
                }
            }
        }
    }
}
