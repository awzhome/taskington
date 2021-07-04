using PPBackup.Base.Config;
using PPBackup.Base.Model;
using PPBackup.Base.Plans;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace PPBackup.WinApp.ViewModel
{
    class MainViewModel : NotifiableObject
    {
        private readonly ConfigurationManager configurationManager;

        public ObservableCollection<BackupPlanViewModel> BackupPlans { get; }

        public MainViewModel(Base.Application application, ConfigurationManager configurationManager, Base.IApplicationEvents applicationEvents)
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
            Application.Current.Dispatcher.Invoke(() =>
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
