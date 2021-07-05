using PPBackup.Base.Config;
using PPBackup.Base.Plans;
using PPBackup.Base.Steps;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PPBackup.Base.Tests
{
    public class YamlConfigurationWriterTests
    {
        [Fact]
        public void PlansWithSteps()
        {
            string yaml =
@"- plan: Test Plan 1
  on: selection
  somekey: somevalue
  steps:
  - sync: dir
    from: path1/path2
    to: path3/path4
  - sync: file
    from: path5/path6/file7
    to: path8/path9/file0
- plan: Test Plan 2
  on: automatically
  steps:
  - sync: dir
    from: path11/path12
    to: path13/path14
";

            var configProvider = new StringConfigurationProvider();
            var configWriter = new YamlConfigurationWriter(configProvider);
            var plans = new[]
            {
                new BackupPlan("selection")
                {
                    Name = "Test Plan 1",
                    ["somekey"] = "somevalue",
                    Steps = new List<BackupStep>(new[]
                    {
                        new BackupStep("sync")
                        {
                            DefaultProperty = "dir",
                            ["from"] = "path1/path2",
                            ["to"] = "path3/path4"
                        },
                        new BackupStep("sync")
                        {
                            DefaultProperty = "file",
                            ["from"] = "path5/path6/file7",
                            ["to"] = "path8/path9/file0"
                        }
                    })
                },
                new BackupPlan("automatically")
                {
                    Name = "Test Plan 2",
                    Steps = new List<BackupStep>(new[]
                    {
                        new BackupStep("sync")
                        {
                            DefaultProperty = "dir",
                            ["from"] = "path11/path12",
                            ["to"] = "path13/path14"
                        }
                    })
                }
            };

            configWriter.Write(plans);
            Assert.Equal(yaml, configProvider.Content);
        }

        [Fact]
        public void PlanWithoutSteps()
        {
            string yaml =
    @"- plan: Test Plan 1
  on: selection
  somekey: somevalue
  steps: []
";

            var configProvider = new StringConfigurationProvider();
            var configWriter = new YamlConfigurationWriter(configProvider);
            var plans = new[]
            {
                new BackupPlan("selection")
                {
                    Name = "Test Plan 1",
                    ["somekey"] = "somevalue",
                    Steps = new List<BackupStep>()
                }
            };

            configWriter.Write(plans);
            Assert.Equal(yaml, configProvider.Content);
        }

        [Fact]
        public void NoPlans()
        {
            string yaml =
    @"[]
";

            var configProvider = new StringConfigurationProvider();
            var configWriter = new YamlConfigurationWriter(configProvider);
            var plans = Enumerable.Empty<BackupPlan>();

            configWriter.Write(plans);
            Assert.Equal(yaml, configProvider.Content);
        }
    }
}
