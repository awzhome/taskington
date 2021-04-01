using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace PPBackup.Gui.ViewModels
{
    public class EditBackupPlanViewModel : ViewModelBase
    {
        public ReactiveCommand<bool, bool> CloseCommand { get; }

        public EditBackupPlanViewModel(BackupPlanViewModel backupPlanViewModel)
        {
            CloseCommand = ReactiveCommand.Create<bool, bool>(save => save);

            InitializeFromBasicModel(backupPlanViewModel);
            SelectedItem = Steps.FirstOrDefault();
        }

        public ObservableCollection<EditBackupStepViewModel> Steps { get; } = new();

        private string? name;
        public string? Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        private EditBackupStepViewModel? selectedItem;
        public EditBackupStepViewModel? SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }

        private void InitializeFromBasicModel(BackupPlanViewModel baseModel)
        {
            name = baseModel.Name;

            foreach (var step in baseModel.Steps)
            {
                Steps.Add(new EditBackupStepViewModel(step));
            }
        }
    }
}
