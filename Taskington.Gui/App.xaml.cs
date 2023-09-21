using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Linq;
using Taskington.Gui.ViewModels;
using Taskington.Gui.Views;

namespace Taskington.Gui
{
    class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var application = new Taskington.Base.Application();
                var guiEnvironment = application.Load(
                        GetType().Assembly,
                        typeof(Taskington.Update.Windows.UpdateServices).Assembly)
                    .OfType<IGuiEnvironment>().FirstOrDefault();

                if (guiEnvironment is not null)
                {
                    var baseEnvironment = application.BaseEnvironment;
                    var mainViewModel = new MainWindowViewModel(
                    baseEnvironment.ConfigurationManager,
                    baseEnvironment.PlanExecution,
                    baseEnvironment.SystemOperations,
                    guiEnvironment.StepUIs);
                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = mainViewModel
                    };
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
