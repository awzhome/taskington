using Avalonia.Threading;
using PPBackup.Base;
using PPBackup.Base.Plans;
using PPBackup.Gui.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PPBackup.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Application application;
        private readonly IEnumerable<ExecutableBackupPlan> executableBackupPlans;
        public ObservableCollection<BackupPlanModel> BackupPlans { get; }

        public MainWindowViewModel(Application application, IEnumerable<ExecutableBackupPlan> executableBackupPlans, IApplicationEvents applicationEvents)
        {
            this.application = application;
            this.executableBackupPlans = executableBackupPlans;

            BackupPlans = new ObservableCollection<BackupPlanModel>();
            applicationEvents.ConfigurationReloaded += (sender, e) =>
            {
                UpdatePlanViewModels();
            };
            UpdatePlanViewModels();
        }

        private void UpdatePlanViewModels()
        {
            Dispatcher.UIThread.Post(() =>
            {
                BackupPlans.Clear();
                foreach (var plan in executableBackupPlans)
                {
                    BackupPlans.Add(new BackupPlanModel(plan));
                }
                application.NotifyInitialStates();
            });
        }
    }
}
