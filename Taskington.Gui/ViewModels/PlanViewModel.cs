using ReactiveUI;
using System.Collections.Generic;
using System.Reactive;
using Taskington.Base.Model;
using Taskington.Base.Plans;
using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    public class PlanViewModel : NotifiableObject
    {
        private readonly ExecutablePlan executablePlan;

        public PlanViewModel(ExecutablePlan executablePlan,
            ReactiveCommand<PlanViewModel, Unit> executePlanCommand,
            ReactiveCommand<PlanViewModel, Unit> editPlanCommand,
            ReactiveCommand<PlanViewModel, Unit> removePlanCommand)
        {
            this.executablePlan = executablePlan;

            ExecutePlanCommand = executePlanCommand;
            EditPlanCommand = editPlanCommand;
            RemovePlanCommand = removePlanCommand;
        }

        public ReactiveCommand<PlanViewModel, Unit> ExecutePlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> EditPlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> RemovePlanCommand { get; }

        public ExecutablePlan ExecutablePlan => executablePlan;

        public IPlanExecution Execution => executablePlan.Execution;

        public IEnumerable<PlanStep> Steps => executablePlan.Plan.Steps;

        public string? Name => executablePlan.Plan.Name;

        private bool isRunning;
        public bool IsRunning
        {
            get => isRunning;
            set
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
            set
            {
                progress = value;
                NotifyPropertyChange();
            }
        }

        private bool hasErrors;
        public bool HasErrors
        {
            get => hasErrors;
            set
            {
                hasErrors = value;
                NotifyPropertyChange();
            }
        }

        private string statusText = "";
        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                NotifyPropertyChange();
            }
        }

        private bool canExecute;
        public bool CanExecute
        {
            get => canExecute;
            set
            {
                canExecute = value;
                NotifyPropertyChange();
                NotifyPropertyChange(nameof(IsPlayable));
            }
        }

        public bool IsPlayable => canExecute && !isRunning;
    }
}
