using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    public static class BackupStepExtensions
    {
        public static EditStepViewModelBase ToViewModel(this PlanStep step, EditBackupPlanViewModel parentModel) => step switch
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
