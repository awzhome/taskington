using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.InnoSetup;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.InnoSetup.InnoSetupTasks;
using static Versioning;
using static Utilities;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath OutputDirectory => RootDirectory / "output";

    BranchConfig BranchVersioningConfig => b => b switch
    {
        "master" => new() { Tag = "preview", IncrementPatch = false },
        _ => new()
    };

    Target ShowVersion => _ => _
        .Executes(() =>
        {
            var version = ProjectVersion(BranchVersioningConfig);
            Console.WriteLine($"Project version is {version.AsString()} (Assembly version: {version.AsAssemblyVersion()})");
        });

    Target Versionize => _ => _
        .DependsOn(ShowVersion)
        .Executes(() =>
        {
            var version = ProjectVersion(BranchVersioningConfig);

            var innoSetupScript = WorkingDirectory / "taskington.iss";
            WriteVersionToFiles("#define APP_VERSION \"$$$\"", version.AsAssemblyVersion(), innoSetupScript);
            WriteVersionToFiles("#define APP_FULL_VERSION \"$$$\"", version.AsString(), innoSetupScript);

            var projectFiles = FindFiles("Taskington*.csproj").Concat(FindFiles("Taskington*.fsproj"));
            WriteVersionToFiles("<Version>$$$</Version>", version.AsString(), projectFiles);
            WriteVersionToFiles("<AssemblyVersion>$$$</AssemblyVersion>", version.AsAssemblyVersion(), projectFiles);
            WriteVersionToFiles("<FileVersion>$$$</FileVersion>", version.AsAssemblyVersion(), projectFiles);

        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target PublishWin64 => _ => _
        .DependsOn(Restore, Versionize)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject("Taskington.Gui")
                .SetConfiguration(Configuration.Release)
                .EnableSelfContained()
                .SetPublishTrimmed(true)
                .SetRuntime("win-x64")
                .SetOutput(OutputDirectory / "publish-win64"));
        });

    Target InstallerWin64 => _ => _
        .DependsOn(PublishWin64)
        .Executes(() =>
        {
            InnoSetup(s => s
                .SetScriptFile("taskington.iss")
                .SetProcessToolPath(((AbsolutePath) SpecialFolder(SpecialFolders.ProgramFilesX86)) / "Inno Setup 6" / "ISCC.exe")
                .SetOutputDir(OutputDirectory));
        });

    Target Test => _ => _
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile("Taskington.Base.Tests")
            );
        });

    Target Win64 => _ => _
        .DependsOn(InstallerWin64);
}