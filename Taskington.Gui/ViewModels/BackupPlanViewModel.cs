using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using Taskington.Base.Model;
using Taskington.Base.Plans;
using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    public class BackupPlanViewModel : NotifiableObject
    {
        private readonly ExecutablePlan executableBackupPlan;

        public BackupPlanViewModel(ExecutablePlan executableBackupPlan,
            ReactiveCommand<BackupPlanViewModel, Unit> executePlanCommand,
            ReactiveCommand<BackupPlanViewModel, Unit> editPlanCommand,
            ReactiveCommand<BackupPlanViewModel, Unit> removePlanCommand)
        {
            this.executableBackupPlan = executableBackupPlan;

            executableBackupPlan.Events.IsRunning += (o, e) => IsRunning = e.IsRunning;
            executableBackupPlan.Events.Progress += (o, e) => Progress = e.Progress;
            executableBackupPlan.Events.StatusText += (o, e) => StatusText = e.StatusText;
            executableBackupPlan.Events.HasErrors += (o, e) => HasErrors = e.HasErrors;
            executableBackupPlan.Events.CanExecute += (o, e) => CanExecute = e.CanExecute;

            ExecutePlanCommand = executePlanCommand;
            EditPlanCommand = editPlanCommand;
            RemovePlanCommand = removePlanCommand;
        }

        public ReactiveCommand<BackupPlanViewModel, Unit> ExecutePlanCommand { get; }
        public ReactiveCommand<BackupPlanViewModel, Unit> EditPlanCommand { get; }
        public ReactiveCommand<BackupPlanViewModel, Unit> RemovePlanCommand { get; }

        public ExecutablePlan ExecutablePlan => executableBackupPlan;

        public IPlanExecution Execution => executableBackupPlan.Execution;

        public IEnumerable<PlanStep> Steps => executableBackupPlan.Plan.Steps;

        public string? Name => executableBackupPlan.Plan.Name;

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
