using PPBackup.Base.Executors;
using PPBackup.Base.Model;

namespace PPBackup.WinApp.ViewModel
{
    class BackupPlanViewModel : NotifiableObject
    {
        private readonly ExecutableBackupPlan executableBackupPlan;

        public BackupPlanViewModel(ExecutableBackupPlan executableBackupPlan)
        {
            this.executableBackupPlan = executableBackupPlan;
        }

        public string Name => executableBackupPlan.BackupPlan.Name;

        private bool isRunning;
        public bool IsRunning
        {
            get => isRunning;
            private set
            {
                isRunning = value;
                NotifyPropertyChange();
            }
        }

        private int progress;
        public int Progress
        {
            get => progress;
            private set
            {
                progress = value;
                NotifyPropertyChange();
            }
        }

        private bool hasErrors;
        public bool HasErrors
        {
            get => hasErrors;
            private set
            {
                hasErrors = value;
                NotifyPropertyChange();
            }
        }

        private string statusText;
        public string StatusText
        {
            get => statusText;
            private set
            {
                statusText = value;
                NotifyPropertyChange();
            }
        }
    }
}
