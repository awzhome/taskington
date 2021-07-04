using PPBackup.Base.Steps;

namespace PPBackup.Gui.ViewModels
{
    public static class BackupStepExtensions
    {
        public static EditStepViewModelBase ToViewModel(this BackupStep step, EditBackupPlanViewModel parentModel) => step switch
        {
            { StepType: "sync" } => new EditSyncStepViewModel(step)
            {
                OpenFolderDialogInteraction = parentModel.OpenFolderDialog,
                OpenFileDialogInteraction = parentModel.OpenFileDialog
            },
            _ => new EditGeneralStepViewModel(step)
        };
    }
}
