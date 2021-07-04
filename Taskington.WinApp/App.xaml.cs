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

            var application = new Base.Application(binder => binder.Bind<MainViewModel>());
            application.Start();

            var mainViewModel = application.ServiceProvider.Get<MainViewModel>();
            var mainView = new MainView
            {
                DataContext = mainViewModel
            };
            mainView.Show();

            application.NotifyInitialStates();
        }
    }
}
