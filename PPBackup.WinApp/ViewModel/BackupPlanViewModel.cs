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

            executableBackupPlan.Events.IsRunningUpdated += (o, e) => IsRunning = e.IsRunning;
            executableBackupPlan.Events.ProgressUpdated += (o, e) => Progress = e.Progress;
            executableBackupPlan.Events.StatusTextUpdated += (o, e) => StatusText = e.StatusText;
            executableBackupPlan.Events.HasErrorsUpdated += (o, e) => HasErrors = e.HasErrors;
            executableBackupPlan.Events.CanExecuteUpdated += (o, e) => CanExecute = e.CanExecute;

            ExecutePlanCommand = new RelayCommand(() => executableBackupPlan.Execution.ExecuteAsync(), () => true);
        }

        public RelayCommand ExecutePlanCommand { get; }

        public string Name => executableBackupPlan.BackupPlan.Name;

        private bool isRunning;
        public bool IsRunning
        {
            get => isRunning;
            private set
            {
                isRunning = value;
                NotifyPropertyChange();
                NotifyPropertyChange(nameof(IsPlayable));
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

        private bool canExecute;
        public bool CanExecute
        {
            get => canExecute;
            private set
            {
                canExecute = value;
                NotifyPropertyChange();
                NotifyPropertyChange(nameof(IsPlayable));
            }
        }

        public bool IsPlayable => canExecute && !isRunning;
    }
}
