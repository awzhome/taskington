using System;
using Taskington.Base.Config;
using Taskington.Base.Plans;
using Xunit;

namespace Taskington.Base.Tests
{
    public class ScriptConfigurationReaderTests
    {
        [Fact]
        public void PlanOneLine()
        {
            string script = @"
plan ""Test Plan 1"" on selection somekey somevalue
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Equal("somevalue", plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                });
        }

        [Fact]
        public void PlanWithLineBreak()
        {
            string script = @"
plan ""Test Plan 1""
    on selection somekey somevalue
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Equal("somevalue", plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                });
        }

        [Fact]
        public void PlanSeparateLinesPerKey()
        {
            string script = @"
plan ""Test Plan 1""
    on selection
    somekey somevalue
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Equal("somevalue", plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                });
        }

        [Fact]
        public void PlanIncompleteKeyValue()
        {
            string script = @"
plan ""Test Plan 1"" on selection somekey
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Null(plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                });
        }

        [Fact]
        public void PlanMissingKey()
        {
            string script = @"
plan ""Test Plan 1"" on selection ""somevalue""
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Null(plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                });
        }

        [Fact]
        public void PlanKeyInQuotes()
        {
            string script = @"
plan ""Test Plan 1"" on selection ""somekey"" somevalue
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Null(plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                });
        }

        [Fact]
        public void ManualPlanWithSteps()
        {
            string script = @"
plan ""Test Plan 1""
    on selection somekey somevalue

sync dir from path1/path2 to path3/path4
sync file from path5/path6/file7 to path8/path9/file0
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
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
                });
        }

        [Fact]
        public void ManualPlanWithMultilineSteps()
        {
            string script = @"
plan ""Test Plan 1""
    on selection somekey somevalue

sync dir
    from path1/path2
    to path3/path4
sync file
    from path5/path6/file7
    to path8/path9/file0
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
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
                });
        }

        [Fact]
        public void PlanOfUnknownTypeWithSteps()
        {
            string script = @"
plan ""Test Plan""
  on SOMETHINGUNKNOWN
sync dir from path1/path2 to path3/path4
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(), plan =>
            {
                Assert.IsType<Plan>(plan);
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
            string script = @"
plan ""Test Plan""
  on selection
SOMETHINGUNKNOWN bla
      from path1/path2
      to path3/path4
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(), plan =>
            {
                Assert.IsType<Plan>(plan);
                Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
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
        public void SingleQuotes()
        {
            string script = @"
plan 'Test Plan 1' on selection somekey ""some value""
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Equal("some value", plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                });
        }

        [Fact]
        public void InvalidWithMixedQuotes()
        {
            string script = @"
plan 'Test Plan 1"" on selection somekey somevalue
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Throws<InvalidOperationException>(() => configReader.Read());
        }

        [Fact]
        public void EmptyValue()
        {
            string script = @"
plan ""Test Plan 1"" on selection somekey """"
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Equal("", plan["somekey"]);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                });
        }

        [Fact]
        public void PlaceholdersWithSpacesWithinQuotes()
        {
            string script = @"
plan ""Test Plan 1""
    on selection
sync dir from path1/path2 to ""${Some Placeholder}/path4""
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                    Assert.Collection(plan.Steps,
                        step =>
                        {
                            Assert.Equal("sync", step.StepType);
                            Assert.Equal("path1/path2", step["from"]);
                            Assert.Equal("${Some Placeholder}/path4", step["to"]);
                            Assert.Null(step["sync"]);
                        });
                });
        }

        [Fact]
        public void PlaceholdersWithSpacesWithoutQuotes()
        {
            string script = @"
plan ""Test Plan 1""
    on selection
sync dir from path1/path2 to ${Some Placeholder}/path4
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Collection(configReader.Read(),
                plan =>
                {
                    Assert.IsType<Plan>(plan);
                    Assert.Equal(Plan.OnSelectionRunType, plan.RunType);
                    Assert.Equal("Test Plan 1", plan.Name);
                    Assert.Null(plan["plan"]);
                    Assert.Null(plan["on"]);
                    Assert.Collection(plan.Steps,
                        step =>
                        {
                            Assert.Equal("sync", step.StepType);
                            Assert.Equal("path1/path2", step["from"]);
                            Assert.Equal("${Some Placeholder}/path4", step["to"]);
                            Assert.Null(step["sync"]);
                        });
                });
        }

        [Fact]
        public void NoPlanHeadCollection()
        {
            string script = @"
sync dir from path1/path2 to path3/path4
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Throws<InvalidOperationException>(() => configReader.Read());
        }

        [Fact]
        public void BadQuoteSyntax()
        {
            string script = @"
plan ""Test Plan
  on SOMETHINGUNKNOWN
sync dir from path1/path2 to path3/path4
";

            var configReader = new ScriptConfigurationReader(new StringConfigurationProvider(script));
            Assert.Throws<InvalidOperationException>(() => configReader.Read());
        }
    }
}
