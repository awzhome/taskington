using ReactiveUI;
using System.Collections.ObjectModel;

namespace PPBackup.Gui.ViewModels
{
    public class EditBackupPlanViewModel : ViewModelBase
    {
        public ReactiveCommand<bool, bool> CloseCommand { get; }

        public EditBackupPlanViewModel(BackupPlanViewModel backupPlanViewModel)
        {
            CloseCommand = ReactiveCommand.Create<bool, bool>(save => save);

            InitializeFromBasicModel(backupPlanViewModel);
        }

        public ObservableCollection<EditBackupStepViewModel> Steps { get; } = new();

        private string? name;
        public string? Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
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
