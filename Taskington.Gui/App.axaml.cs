using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
                application.Load(GetType().Assembly, typeof(Taskington.Update.Windows.UpdateServices).Assembly);

                var mainViewModel = new MainWindowViewModel();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
