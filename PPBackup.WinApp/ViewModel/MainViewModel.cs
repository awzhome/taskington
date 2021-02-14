using PPBackup.Base.Model;
using PPBackup.Base.Plans;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace PPBackup.WinApp.ViewModel
{
    class MainViewModel : NotifiableObject
    {
        private readonly IEnumerable<ExecutableBackupPlan> executableBackupPlans;

        public ObservableCollection<BackupPlanViewModel> BackupPlans { get; }

        public MainViewModel(Base.Application application, IEnumerable<ExecutableBackupPlan> executableBackupPlans, Base.IApplicationEvents applicationEvents)
        {
            this.executableBackupPlans = executableBackupPlans;

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
                foreach (var plan in executableBackupPlans)
                {
                    BackupPlans.Add(new BackupPlanViewModel(plan));
                }
            });
        }
    }
}
