using System;
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

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath OutputDirectory => RootDirectory / "output";

    Func<string, BranchVersioning> BranchVersioningConfig => b => b switch
    {
        "master" => new() { Tag = "preview", IncrementPatch = false },
        _ => new()
    };

    Target builddebug => _ => _
        .Executes(() =>
        {
            string gittag;
            BuildVersion version;

            gittag = "dev-3.0-rc-23-abdcefg";
            version = GitTagParser.Parse(gittag);
            Console.WriteLine($"{gittag} -> {version.AsString()} | {version.AsAssemblyVersion()}");

            version = ProjectVersion(BranchVersioningConfig, "63446b8");
            Console.WriteLine($"Project version is {version.AsString()} | {version.AsAssemblyVersion()}");
        });

    Target clean => _ => _
        .Before(restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
        });

    Target restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target compile => _ => _
        .DependsOn(restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target publish_win64 => _ => _
        .DependsOn(restore)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject("PPBackup.Gui")
                .SetConfiguration(Configuration.Release)
                .EnableSelfContained()
                .SetPublishTrimmed(true)
                .SetRuntime("win-x64")
                .SetOutput(OutputDirectory / "publish-win64"));
        });

    Target installer_win64 => _ => _
        .DependsOn(publish_win64)
        .Executes(() =>
        {
            InnoSetup(s => s
                .SetScriptFile("ppbackup.iss")
                .SetProcessToolPath(((AbsolutePath) SpecialFolder(SpecialFolders.ProgramFilesX86)) / "Inno Setup 6" / "ISCC.exe")
                .SetOutputDir(OutputDirectory));
        });

    Target test => _ => _
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile("PPBackup.Base.Tests")
            );
        });

    Target win64 => _ => _
        .DependsOn(installer_win64);
}
