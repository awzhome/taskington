using System.Linq;
using System.Reflection;

namespace Taskington.Gui;

static class AppInfo
{
    public const string Copyright = "Copyright (c) Andreas Weizel";

    public static string Version => Assembly.GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion.Split('+').FirstOrDefault() ?? "local";
}