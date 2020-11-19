using PPBackup.Base;
using PPBackup.Base.Executors;
using PPBackup.Base.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PPBackup.WinApp.ViewModel
{
    public class MainViewModel : NotifiableObject
    {
        private readonly DialogController dialogController;

        public ObservableCollection<BackupPlanViewModel> BackupPlans { get; }

        public MainViewModel(Application application)
        {
            dialogController = new DialogController(this);

            var executableBackupPlans = application.Services.Get<IEnumerable<ExecutableBackupPlan>>();
            BackupPlans = new ObservableCollection<BackupPlanViewModel>(
                executableBackupPlans?.Select(plan =>
                    new BackupPlanViewModel(plan, dialogController))
                ?? Array.Empty<BackupPlanViewModel>());
        }
    }
}
