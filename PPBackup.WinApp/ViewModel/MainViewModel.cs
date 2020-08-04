using PPBackup.Base;
using PPBackup.Base.Executors;
using PPBackup.Base.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PPBackup.WinApp.ViewModel
{
    class MainViewModel : NotifiableObject
    {
        public ObservableCollection<BackupPlanViewModel> BackupPlans { get; }

        public MainViewModel(Application application)
        {
            var executableBackupPlans = application.Services.Get<IEnumerable<ExecutableBackupPlan>>();
            BackupPlans = new ObservableCollection<BackupPlanViewModel>(executableBackupPlans?.Select(plan => new BackupPlanViewModel(plan)));
        }
    }
}
