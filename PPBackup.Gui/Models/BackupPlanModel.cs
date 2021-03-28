using PPBackup.Base.Model;
using PPBackup.Base.Plans;
using ReactiveUI;
using System.Reactive;

namespace PPBackup.Gui.Models
{
    public class BackupPlanModel : NotifiableObject
    {
        private readonly ExecutableBackupPlan executableBackupPlan;

        public BackupPlanModel(ExecutableBackupPlan executableBackupPlan)
        {
            this.executableBackupPlan = executableBackupPlan;

            executableBackupPlan.Events.IsRunning += (o, e) => IsRunning = e.IsRunning;
            executableBackupPlan.Events.Progress += (o, e) => Progress = e.Progress;
            executableBackupPlan.Events.StatusText += (o, e) => StatusText = e.StatusText;
            executableBackupPlan.Events.HasErrors += (o, e) => HasErrors = e.HasErrors;
            executableBackupPlan.Events.CanExecute += (o, e) => CanExecute = e.CanExecute;

            ExecutePlanCommand = ReactiveCommand.CreateFromTask(executableBackupPlan.Execution.ExecuteAsync);
        }

        public ReactiveCommand<Unit, Unit> ExecutePlanCommand { get; }

        public string? Name => executableBackupPlan.BackupPlan.Name;

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

        private string statusText = "";
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
