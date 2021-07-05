using Taskington.WinApp.View;
using System.Windows;
using Taskington.WinApp.ViewModel;

namespace Taskington.WinApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var application = new Taskington.Base.Application(binder => binder.Bind<MainViewModel>());
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
