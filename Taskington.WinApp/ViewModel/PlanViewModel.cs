using Taskington.Base.Model;
using Taskington.Base.Plans;
using Taskington.WinApp.ViewModel;

namespace Taskington.WinApp.ViewModel
{
    class PlanViewModel : NotifiableObject
    {
        private readonly ExecutablePlan executablePlan;

        public PlanViewModel(ExecutablePlan executablePlan)
        {
            this.executablePlan = executablePlan;

            this.executablePlan.Events.IsRunning += (o, e) => IsRunning = e.IsRunning;
            this.executablePlan.Events.Progress += (o, e) => Progress = e.Progress;
            this.executablePlan.Events.StatusText += (o, e) => StatusText = e.StatusText;
            this.executablePlan.Events.HasErrors += (o, e) => HasErrors = e.HasErrors;
            this.executablePlan.Events.CanExecute += (o, e) => CanExecute = e.CanExecute;

            ExecutePlanCommand = new RelayCommand(() => executablePlan.Execution.ExecuteAsync(), () => true);
        }

        public RelayCommand ExecutePlanCommand { get; }

        public string Name => executablePlan.Plan.Name;

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
