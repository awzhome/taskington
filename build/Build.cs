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
using static Utilities;
using AWZhome.GutenTag;
using AWZhome.GutenTag.Nuke;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    string WinInstallerSolution => RootDirectory / "Taskington.Installer.Windows.sln"; 

    AbsolutePath OutputDirectory => RootDirectory / "output";

    BranchSpecificConfig BranchVersioningConfig => b => b switch
    {
        "master" => new() { Tag = "preview", IncrementedPart = IncrementedPart.Minor },
        _ => new()
    };

    Versioning Versioning => new(BranchVersioningConfig, new NukeGitAdapter(new VersioningConfig()));

    Target ShowVersion => _ => _
        .Executes(() =>
        {
            var version = Versioning.GetVersionInfo();
            Console.WriteLine($"Project version is {version.AsString()} (numeric: {version.AsNumericVersion()})");
        });

    Target Versionize => _ => _
        .DependsOn(ShowVersion)
        .Executes(() =>
        {
            var version = Versioning.GetVersionInfo();
            var writer = new VersionInfoWriter(version);

            writer.WriteToVsProject(FindFiles("Taskington*.csproj").Concat(FindFiles("Taskington*.fsproj")));

            VersionInfoWriter.WriteVersionToFiles(
                "public static string NumericVersion = \"$$$\";", 
                version.AsNumericVersion(),
                WorkingDirectory / "Taskington.Installer.Windows" / "AppPackage.cs");
            VersionInfoWriter.WriteVersionToFiles(
                "public static string FullVersion = \"$$$\";", 
                version.AsString(),
                WorkingDirectory / "Taskington.Installer.Windows" / "AppPackage.cs");

            VersionInfoWriter.WriteVersionToFiles(
                "public static string Version = \"$$$\";", 
                version.AsString(), 
                WorkingDirectory / "Taskington.Gui" / "AppInfo.cs");
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
            DotNetBuild(s => s
                .SetProjectFile(WinInstallerSolution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
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
