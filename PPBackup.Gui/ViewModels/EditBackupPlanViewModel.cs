using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace PPBackup.Gui.ViewModels
{
    public class EditBackupPlanViewModel : ViewModelBase
    {
        public ReactiveCommand<bool, bool> CloseCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveStepCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveStepUpCommand { get; }
        public ReactiveCommand<Unit, Unit> MoveStepDownCommand { get; }

        public EditBackupPlanViewModel(BackupPlanViewModel backupPlanViewModel)
        {
            CloseCommand = ReactiveCommand.Create<bool, bool>(save => save);

            RemoveStepCommand = ReactiveCommand.Create(RemoveStep,
                this.WhenAnyValue(x => x.SelectedItem, (EditStepViewModelBase? selectedItem) => selectedItem != null));
            MoveStepUpCommand = ReactiveCommand.Create(MoveStepUp,
                this.WhenAnyValue(x => x.SelectedItem, selectedItem => Steps.FirstOrDefault() != selectedItem));
            MoveStepDownCommand = ReactiveCommand.Create(MoveStepDown,
                this.WhenAnyValue(x => x.SelectedItem, selectedItem => Steps.LastOrDefault() != selectedItem));

            InitializeFromBasicModel(backupPlanViewModel);
            SelectedItem = Steps.FirstOrDefault();
        }

        public ObservableCollection<EditStepViewModelBase> Steps { get; } = new();

        private string? name;
        public string? Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        private EditStepViewModelBase? selectedItem;
        public EditStepViewModelBase? SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                this.RaisePropertyChanged();
            }
        }

        private void InitializeFromBasicModel(BackupPlanViewModel baseModel)
        {
            name = baseModel.Name;

            foreach (var step in baseModel.Steps)
            {
                Steps.Add(step.ToViewModel());
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
                    SelectedItem = null;
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
                    SelectedItem = null;
                    SelectedItem = Steps[selectedIndex + 1];
                }
            }
        }
    }
}
