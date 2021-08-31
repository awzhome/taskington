using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Taskington.Base.Plans;
using Taskington.Base.Service;
using Taskington.Base.Steps;
using Taskington.Gui.UIProviders;

namespace Taskington.Gui.ViewModels
{
    public class EditPlanViewModel : ViewModelBase
    {
        private readonly Plan plan;
        private readonly IAppServiceProvider serviceProvider;

        public ReactiveCommand<bool, bool> CloseCommand { get; }
        public ReactiveCommand<NewStepTemplate, Unit> AddStepCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveStepCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveStepUpCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveStepDownCommand { get; }
        public Interaction<Unit, string> OpenFolderDialog { get; }
        public Interaction<Unit, string?> OpenFileDialog { get; }

        public EditPlanViewModel(IAppServiceProvider serviceProvider, PlanViewModel planViewModel)
        {
            this.serviceProvider = serviceProvider;
            plan = planViewModel.ExecutablePlan.Plan;

            CloseCommand = ReactiveCommand.Create<bool, bool>(save => save);

            AddStepCommand = ReactiveCommand.Create<NewStepTemplate>(AddStep);
            RemoveStepCommand = ReactiveCommand.Create(RemoveStep);
            MoveStepUpCommand = ReactiveCommand.Create(MoveStepUp);
            MoveStepDownCommand = ReactiveCommand.Create(MoveStepDown);

            OpenFolderDialog = new();
            OpenFileDialog = new();

            InitializeFromBasicModel();
            SelectedItem = Steps.FirstOrDefault();

            NewStepTemplates = CollectNewStepTemplates();
        }

        public ObservableCollection<EditStepViewModelBase> Steps { get; } = new();

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

        private EditStepViewModelBase? selectedItem;
        public EditStepViewModelBase? SelectedItem
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

        private EditStepViewModelBase CreateEditStepViewModel(PlanStep step) =>
            serviceProvider.Get<IStepTypeUI>(stepTypeUI => stepTypeUI.StepType == step.StepType)
                ?.CreateEditViewModel(step, this)
                ?? new EditGeneralStepViewModel(step);

        private List<NewStepTemplate> CollectNewStepTemplates() =>
            new(serviceProvider.GetAll<IStepTypeUI>().SelectMany(stepTypeUI => stepTypeUI.GetNewStepTemplates()));

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
                if (stepEditModel != null)
                {
                    Steps.Add(stepEditModel);
                    SelectedItem = stepEditModel;
                }
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
