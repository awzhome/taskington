using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Taskington.Base.Plans;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;
using Taskington.Gui.Extension;
using Taskington.Gui.Extension.Events;

namespace Taskington.Gui.ViewModels
{
    class EditPlanViewModel : ViewModelBase, IEditPlanViewModel
    {
        private readonly Plan plan;
        private readonly Placeholders placeholders;

        public ReactiveCommand<bool, bool> CloseCommand { get; }
        public ReactiveCommand<NewStepTemplate, Unit> AddStepCommand { get; }
        public ReactiveCommand<Unit, Unit> DuplicateStepCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveStepCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveStepUpCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveStepDownCommand { get; }
        public Interaction<Unit, string?> OpenFolderDialog { get; }
        public Interaction<Unit, string?> OpenFileDialog { get; }

        public EditPlanViewModel(PlanViewModel planViewModel)
        {
            plan = planViewModel.Plan;
            placeholders = SystemOperationsEvents.LoadSystemPlaceholders.Request().First();

            CloseCommand = ReactiveCommand.Create<bool, bool>(save => save);

            AddStepCommand = ReactiveCommand.Create<NewStepTemplate>(AddStep);
            DuplicateStepCommand = ReactiveCommand.Create(DuplicateStep,
                this.WhenAnyValue(x => x.SelectedItem, (IEditStepViewModel? selectedItem) => selectedItem != null));
            RemoveStepCommand = ReactiveCommand.Create(RemoveStep,
                this.WhenAnyValue(x => x.SelectedItem, (IEditStepViewModel? selectedItem) => selectedItem != null));
            MoveStepUpCommand = ReactiveCommand.Create(MoveStepUp,
                this.WhenAnyValue(x => x.SelectedItem, (IEditStepViewModel? selectedItem) => Steps.FirstOrDefault() != selectedItem));
            MoveStepDownCommand = ReactiveCommand.Create(MoveStepDown,
                this.WhenAnyValue(x => x.SelectedItem, (IEditStepViewModel? selectedItem) => Steps.LastOrDefault() != selectedItem));

            OpenFolderDialog = new();
            OpenFileDialog = new();

            InitializeFromBasicModel();
            SelectedItem = Steps.FirstOrDefault();

            NewStepTemplates = CollectNewStepTemplates();
        }

        public ObservableCollection<IEditStepViewModel> Steps { get; } = new();

        public List<NewStepTemplate> NewStepTemplates { get; }

        private string? name;
        public string? Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        private string? runType;
        public string? RunType
        {
            get => runType;
            set => this.RaiseAndSetIfChanged(ref runType, value);
        }

        private IEditStepViewModel? selectedItem;
        public IEditStepViewModel? SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = null;
                this.RaisePropertyChanged();
                if (value != null)
                {
                    selectedItem = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private IEditStepViewModel CreateEditStepViewModel(PlanStep step) =>
            StepUIEvents.NewEditViewModel.Request(step, this, placeholders).FirstOrDefault() ?? new EditGeneralStepViewModel(step);

        private List<NewStepTemplate> CollectNewStepTemplates() =>
            new(StepUIEvents.NewStepTemplates.RequestMany());

        private void InitializeFromBasicModel()
        {
            name = plan.Name;
            runType = plan.RunType;

            foreach (var step in plan.Steps)
            {
                Steps.Add(CreateEditStepViewModel(step));
            }
        }

        public Plan ConvertToPlan()
        {
            Plan newPlan = new(runType ?? Plan.OnSelectionRunType, plan.Properties)
            {
                Name = name,
                Steps = Steps.Select(step => step.ConvertToStep())
            };

            return newPlan;
        }

        private void AddStep(NewStepTemplate template)
        {
            var newStep = template?.Creator?.Invoke();
            if (newStep != null)
            {
                var stepEditModel = CreateEditStepViewModel(newStep);
                Steps.Add(stepEditModel);
                SelectedItem = stepEditModel;
            }
        }

        private void DuplicateStep()
        {
            if (SelectedItem != null)
            {
                var newStep = SelectedItem.ConvertToStep();
                int selectedIndex = Steps.IndexOf(SelectedItem);
                var stepEditModel = CreateEditStepViewModel(newStep);
                Steps.Insert(selectedIndex + 1, stepEditModel);
                SelectedItem = stepEditModel;
            }
        }

        private void RemoveStep()
        {
            if (SelectedItem != null)
            {
                int selectedIndex = Steps.IndexOf(SelectedItem);
                Steps.Remove(SelectedItem);
                if (Steps.Count > 0)
                {
                    if (Steps.Count <= selectedIndex)
                    {
                        selectedIndex = Steps.Count - 1;
                    }
                    SelectedItem = Steps[selectedIndex];
                }
                else
                {
                    SelectedItem = null;
                }
            }
        }

        private void MoveStepUp()
        {
            if (SelectedItem != null)
            {
                int selectedIndex = Steps.IndexOf(SelectedItem);
                if (selectedIndex > 0)
                {
                    Steps.Move(selectedIndex, selectedIndex - 1);
                    SelectedItem = Steps[selectedIndex - 1];
                }
            }
        }

        private void MoveStepDown()
        {
            if (SelectedItem != null)
            {
                int selectedIndex = Steps.IndexOf(SelectedItem);
                if (selectedIndex < Steps.Count - 1)
                {
                    Steps.Move(selectedIndex, selectedIndex + 1);
                    SelectedItem = Steps[selectedIndex + 1];
                }
            }
        }
    }
}
