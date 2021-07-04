using PPBackup.Base.Config;
using PPBackup.Base.Plans;
using System;
using Xunit;

namespace PPBackup.Base.Tests
{
    public class YamlConfigurationReaderTests
    {
        [Fact]
        public void PlansWithSteps()
        {
            string yaml = @"
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
  on: start
  steps:
    - sync: dir
      from: path11/path12
      to: path13/path14
";

            var configReader = new YamlConfigurationReader(new StringConfigurationProvider(yaml));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<BackupPlan>(plan);
                    Assert.Equal(BackupPlan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Equal("somevalue", plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                    Assert.Collection(plan.Steps,
                        step =>
                        {
                            Assert.Equal("sync", step.StepType);
                            Assert.Equal("path1/path2", step["from"]);
                            Assert.Equal("path3/path4", step["to"]);
                            Assert.Null(step["sync"]);
                        },
                        step =>
                        {
                            Assert.Equal("sync", step.StepType);
                            Assert.Equal("path5/path6/file7", step["from"]);
                            Assert.Equal("path8/path9/file0", step["to"]);
                        });
                },
                plan =>
                {
                    Assert.IsType<BackupPlan>(plan);
                    Assert.Equal("start", plan.RunType);
                    Assert.Equal("Test Plan 2", plan.Name);
                    Assert.Collection(plan.Steps,
                        step =>
                        {
                            Assert.Equal("sync", step.StepType);
                            Assert.Equal("path11/path12", step["from"]);
                            Assert.Equal("path13/path14", step["to"]);
                        });
                });
        }

        [Fact]
        public void PlanOfUnknownTypeWithSteps()
        {
            string yaml = @"
- plan: Test Plan
  on: SOMETHINGUNKNOWN
  steps:
    - sync: dir
      from: path1/path2
      to: path3/path4
";

            var configReader = new YamlConfigurationReader(new StringConfigurationProvider(yaml));
            Assert.Collection(configReader.Read(), plan =>
            {
                Assert.IsType<BackupPlan>(plan);
                Assert.Equal("SOMETHINGUNKNOWN", plan.RunType);
                Assert.Equal("Test Plan", plan.Name);
                Assert.Collection(plan.Steps,
                    step =>
                    {
                        Assert.Equal("sync", step.StepType);
                        Assert.Equal("path1/path2", step["from"]);
                        Assert.Equal("path3/path4", step["to"]);
                    });
            });
        }

        [Fact]
        public void PlanWithStepsOfUnknownType()
        {
            string yaml = @"
- plan: Test Plan
  on: selection
  steps:
    - SOMETHINGUNKNOWN: bla
      from: path1/path2
      to: path3/path4
";

            var configReader = new YamlConfigurationReader(new StringConfigurationProvider(yaml));
            Assert.Collection(configReader.Read(), plan =>
            {
                Assert.IsType<BackupPlan>(plan);
                Assert.Equal(BackupPlan.OnSelectionRunType, plan.RunType);
                Assert.Equal("Test Plan", plan.Name);
                Assert.Collection(plan.Steps,
                    step =>
                    {
                        Assert.Equal("SOMETHINGUNKNOWN", step.StepType);
                        Assert.Equal("path1/path2", step["from"]);
                        Assert.Equal("path3/path4", step["to"]);
                    });
            });
        }

        [Fact]
        public void WrongYamlStructure()
        {
            string yaml = @"
plan: Test Plan
on: SOMETHINGUNKNOWN
steps:
  - sync: dir
    from: path1/path2
    to: path3/path4
";

            var configReader = new YamlConfigurationReader(new StringConfigurationProvider(yaml));
            Assert.Empty(configReader.Read());
        }

        [Fact]
        public void WrongYamlStructure_SequenceAsPropertyValue()
        {
            string yaml = @"
- plan: Test Plan
  on: selection
  somekey:
    - bla
    - blubb
";

            var configReader = new YamlConfigurationReader(new StringConfigurationProvider(yaml));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<BackupPlan>(plan);
                    Assert.Equal(BackupPlan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan", plan.Name);
                    Assert.Null(plan["somekey"]);
                });
        }

        [Fact]
        public void SyntaxErrors()
        {
            string yaml = @"
-234plan; Test Plan
          on: SOMETHINGUNKNOWN
steps=
  - sync: dir
    from: path1/path2
    to: path3/path4
";

            var configReader = new YamlConfigurationReader(new StringConfigurationProvider(yaml));
            Assert.Throws<InvalidOperationException>(() => configReader.Read());
        }
    }
}
