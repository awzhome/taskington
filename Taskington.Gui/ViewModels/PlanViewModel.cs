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
            ReactiveCommand<PlanViewModel, Unit> removePlanCommand,
            ReactiveCommand<PlanViewModel, Unit> undoPlanRemovalCommand)
        {
            this.executablePlan = executablePlan;

            ExecutePlanCommand = executePlanCommand;
            EditPlanCommand = editPlanCommand;
            RemovePlanCommand = removePlanCommand;
            UndoPlanRemovalCommand = undoPlanRemovalCommand;
        }

        public ReactiveCommand<PlanViewModel, Unit> ExecutePlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> EditPlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> RemovePlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> UndoPlanRemovalCommand { get; }

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
                SetAndNotify(ref isRunning, value);
                NotifyPropertyChange(nameof(IsPlayable));
            }
        }

        private int progress;
        public int Progress
        {
            get => progress;
            set => SetAndNotify(ref progress, value);
        }

        private bool hasErrors;
        public bool HasErrors
        {
            get => hasErrors;
            set => SetAndNotify(ref hasErrors, value);
        }

        private string statusText = "";
        public string StatusText
        {
            get => statusText;
            set => SetAndNotify(ref statusText, value);
        }

        private bool canExecute;
        public bool CanExecute
        {
            get => canExecute;
            set
            {
                SetAndNotify(ref canExecute, value);
                NotifyPropertyChange(nameof(IsPlayable));
            }
        }

        public bool IsPlayable => canExecute && !isRunning;

        private bool isRemoved;
        public bool IsRemoved
        {
            get => isRemoved;
            set => SetAndNotify(ref isRemoved, value);
        }

        public int PreviousIndex { get; set; }
    }
}
