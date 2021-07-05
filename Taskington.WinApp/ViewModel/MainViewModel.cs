using Taskington.WinApp.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Taskington.Base;
using Taskington.Base.Config;
using Taskington.Base.Model;

namespace Taskington.WinApp.ViewModel
{
    class MainViewModel : NotifiableObject
    {
        private readonly ConfigurationManager configurationManager;

        public ObservableCollection<BackupPlanViewModel> BackupPlans { get; }

        public MainViewModel(Base.Application application, ConfigurationManager configurationManager, IApplicationEvents applicationEvents)
        {
            this.configurationManager = configurationManager;

            BackupPlans = new ObservableCollection<BackupPlanViewModel>();
            applicationEvents.ConfigurationReloaded += (sender, e) =>
            {
                UpdatePlanViewModels();
                application.NotifyInitialStates();
            };
            UpdatePlanViewModels();
        }

        private void UpdatePlanViewModels()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                BackupPlans.Clear();
                foreach (var plan in configurationManager.ExecutablePlans)
                {
                    BackupPlans.Add(new BackupPlanViewModel(plan));
                }
            });
        }
    }
}
