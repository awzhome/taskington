using System.Collections.Generic;
using System.Linq;
using Taskington.Base.Config;
using Taskington.Base.Plans;
using Taskington.Base.Steps;
using Xunit;

namespace Taskington.Base.Tests
{
    public class YamlConfigurationWriterTests
    {
        [Fact]
        public void ConfigValues()
        {
            string yaml =
    @"config:
  key1: value1
  key2: value2
  key3: value3
  key4: ''
plans: []
";

            var configProvider = new StringConfigurationProvider();
            var configWriter = new YamlConfigurationWriter(configProvider);
            var configValues = new (string, string?)[]
            {
                ("key1", "value1"),
                ("key2", "value2"),
                ("key3", "value3"),
                ("key4", null)
            };

            configWriter.Write(new Configuration(configValues, Enumerable.Empty<Plan>()));
            Assert.Equal(yaml, configProvider.Content);
        }

        [Fact]
        public void PlansWithSteps()
        {
            string yaml =
@"config: {}
plans:
- plan: Test Plan 1
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
                new Plan("selection")
                {
                    Name = "Test Plan 1",
                    ["somekey"] = "somevalue",
                    Steps = new List<PlanStep>(new[]
                    {
                        new PlanStep("sync")
                        {
                            DefaultProperty = "dir",
                            ["from"] = "path1/path2",
                            ["to"] = "path3/path4"
                        },
                        new PlanStep("sync")
                        {
                            DefaultProperty = "file",
                            ["from"] = "path5/path6/file7",
                            ["to"] = "path8/path9/file0"
                        }
                    })
                },
                new Plan("automatically")
                {
                    Name = "Test Plan 2",
                    Steps = new List<PlanStep>(new[]
                    {
                        new PlanStep("sync")
                        {
                            DefaultProperty = "dir",
                            ["from"] = "path11/path12",
                            ["to"] = "path13/path14"
                        }
                    })
                }
            };

            configWriter.Write(new Configuration(Enumerable.Empty<(string, string?)>(), plans));
            Assert.Equal(yaml, configProvider.Content);
        }

        [Fact]
        public void PlanWithoutSteps()
        {
            string yaml =
    @"config: {}
plans:
- plan: Test Plan 1
  on: selection
  somekey: somevalue
  steps: []
";

            var configProvider = new StringConfigurationProvider();
            var configWriter = new YamlConfigurationWriter(configProvider);
            var plans = new[]
            {
                new Plan("selection")
                {
                    Name = "Test Plan 1",
                    ["somekey"] = "somevalue",
                    Steps = new List<PlanStep>()
                }
            };

            configWriter.Write(new Configuration(Enumerable.Empty<(string, string?)>(), plans));
            Assert.Equal(yaml, configProvider.Content);
        }

        [Fact]
        public void NoPlans()
        {
            string yaml =
    @"config: {}
plans: []
";

            var configProvider = new StringConfigurationProvider();
            var configWriter = new YamlConfigurationWriter(configProvider);
            var plans = Enumerable.Empty<Plan>();

            configWriter.Write(new Configuration(Enumerable.Empty<(string, string?)>(), plans));
            Assert.Equal(yaml, configProvider.Content);
        }
    }
}
