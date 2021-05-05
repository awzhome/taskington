using PPBackup.Base.Plans;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace PPBackup.Gui.ViewModels
{
    public class EditBackupPlanViewModel : ViewModelBase
    {
        private readonly BackupPlan plan;

        public ReactiveCommand<bool, bool> CloseCommand { get; }
        public ReactiveCommand<NewStepTemplate, Unit> AddStepCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveStepCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveStepUpCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveStepDownCommand { get; }
        public Interaction<Unit, string> OpenFolderDialog { get; }
        public Interaction<Unit, string?> OpenFileDialog { get; }

        public EditBackupPlanViewModel(BackupPlanViewModel backupPlanViewModel)
        {
            plan = backupPlanViewModel.ExecutablePlan.BackupPlan;

            CloseCommand = ReactiveCommand.Create<bool, bool>(save => save);

            AddStepCommand = ReactiveCommand.Create<NewStepTemplate>(AddStep);
            RemoveStepCommand = ReactiveCommand.Create(RemoveStep,
                this.WhenAnyValue(x => x.SelectedItem, (EditStepViewModelBase? selectedItem) => selectedItem != null));
            MoveStepUpCommand = ReactiveCommand.Create(MoveStepUp,
                this.WhenAnyValue(x => x.SelectedItem, selectedItem => Steps.FirstOrDefault() != selectedItem));
            MoveStepDownCommand = ReactiveCommand.Create(MoveStepDown,
                this.WhenAnyValue(x => x.SelectedItem, selectedItem => Steps.LastOrDefault() != selectedItem));

            OpenFolderDialog = new();
            OpenFileDialog = new();

            InitializeFromBasicModel();
            SelectedItem = Steps.FirstOrDefault();
        }

        public ObservableCollection<EditStepViewModelBase> Steps { get; } = new();

        public NewStepTemplates NewStepTemplates { get; } = new();

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

        private void InitializeFromBasicModel()
        {
            name = plan.Name;
            runType = plan.RunType;

            foreach (var step in plan.Steps)
            {
                Steps.Add(step.ToViewModel(this));
            }
        }

        public BackupPlan ConvertToPlan()
        {
            BackupPlan newPlan = new(runType ?? BackupPlan.OnSelectionRunType, plan.Properties)
            {
                Name = name,
                Steps = Steps.Select(step => step.ConvertToStep())
            };

            return newPlan;
        }

        private void AddStep(NewStepTemplate template)
        {
            var newStep = template?.Creator?.Invoke()?.ToViewModel(this);
            if (newStep != null)
            {
                Steps.Add(newStep);
                SelectedItem = newStep;
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
