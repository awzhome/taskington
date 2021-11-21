using System;
using System.IO;
using System.Linq;

using WixSharp;

namespace Taskington.Installer.Windows
{
    internal class Builder
    {
        static public void Main()
        {
            Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories = true;

            var buildOutputDir = $@"output\publish-win64";

            var project = new Project("Taskington",
                              new InstallDir(@"%LocalAppData%\Programs\Taskington\bin",
                                  new DirFiles(Path.Combine(buildOutputDir, "*.*"))));

            project.GUID = new Guid("91c41e60-c478-4ce8-a299-568bd3258b20");
            project.Version = new Version(AppPackage.NumericVersion);
            project.SourceBaseDir = Path.GetDirectoryName(Environment.CurrentDirectory);
            project.InstallScope = InstallScope.perUser;
            project.InstallPrivileges = InstallPrivileges.limited;

            project.MajorUpgrade = new MajorUpgrade
            {
                Schedule = UpgradeSchedule.afterInstallInitialize,
                AllowSameVersionUpgrades = true,
                DowngradeErrorMessage = "A newer release of Taskington is already installed on this system. Please uninstall it first to continue."
            };

            project.UI = WUI.WixUI_ProgressOnly;

            project.ResolveWildCards().FindFile(f => f.Name.EndsWith("taskington-gui.exe")).First()
                .Shortcuts = new[] {
                    new FileShortcut("Taskington", @"%ProgramMenu%")
                };

            Compiler.BuildMsi(project, Path.Combine(project.SourceBaseDir, "output", $"taskington-x64-{AppPackage.FullVersion}.msi"));
        }
    }
}
