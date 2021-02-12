using PPBackup.Base.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PPBackup.Base.Config
{
    public class ScriptConfigurationReader
    {
        private readonly IStreamReaderProvider configurationProvider;

        public ScriptConfigurationReader(IStreamReaderProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public IEnumerable<BackupPlan> Read()
        {
            List<BackupPlan> plans = new List<BackupPlan>();

            configurationProvider.ReadConfigurationStreams(reader =>
            {
                List<BackupStep> steps = new();
                BackupPlan backupPlan = new(BackupPlan.OnSelectionRunType)
                {
                    Steps = steps
                };

                bool beginOfFile = true;
                bool isFirstCollection = true;
                BackupStep? lastStep = null;
                foreach ((var key, var value, var newCollection) in ParseStream(reader))
                {
                    if (beginOfFile && !newCollection)
                    {
                        throw new InvalidOperationException("Faulty backup plan configuration.");
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
                                backupPlan.Name = value;
                            }
                            else
                            {
                                throw new InvalidOperationException("Faulty backup plan configuration.");
                            }
                        }
                        else
                        {
                            if (key == "on")
                            {
                                backupPlan.RunType = value;
                            }
                            else
                            {
                                backupPlan[key] = value;
                            }
                        }
                    }
                    else
                    {
                        if (newCollection)
                        {
                            lastStep = new BackupStep(key)
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

                plans.Add(backupPlan);
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
