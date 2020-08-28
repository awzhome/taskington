using PPBackup.WinApp.View;
using PPBackup.WinApp.ViewModel;
using System.Windows;

namespace PPBackup.WinApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var application = new Base.Application();
            application.Start();

            var mainViewModel = new MainViewModel(application);

            var mainView = new MainView();
            mainView.DataContext = mainViewModel;
            mainView.Show();

            application.NotifyInitialStates();
        }
    }
}
