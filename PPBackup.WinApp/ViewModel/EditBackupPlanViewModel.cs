using MahApps.Metro.Controls.Dialogs;
using PPBackup.Base.Model;
using System.Collections.Generic;

namespace PPBackup.WinApp.ViewModel
{
    public class EditBackupPlanViewModel : NotifiableObject
    {
        private readonly BackupPlanViewModel backupPlanViewModel;

        public EditBackupPlanViewModel(BackupPlanViewModel backupPlanViewModel)
        {
            this.backupPlanViewModel = backupPlanViewModel;
            CloseDialogCommand = backupPlanViewModel.CloseEditDialogCommand;

            name = backupPlanViewModel.Name ?? string.Empty;
            runType = backupPlanViewModel.ExecutableBackupPlan.BackupPlan.RunType;
        }

        public AsyncRelayCommand<BaseMetroDialog> CloseDialogCommand { get; }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChange();
            }
        }

        private string runType;
        public string RunType
        {
            get => runType;
            set
            {
                runType = value;
                NotifyPropertyChange();
            }
        }

        public IEnumerable<string> AvailableRunTypes => new[] { "manually", "automatically" };
    }
}
