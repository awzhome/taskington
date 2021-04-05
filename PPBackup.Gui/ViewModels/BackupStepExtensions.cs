using PPBackup.Base.Steps;

namespace PPBackup.Gui.ViewModels
{
    public static class BackupStepExtensions
    {
        public static EditStepViewModelBase ToViewModel(this BackupStep step) => step switch
        {
            { StepType: "sync" } => new EditSyncStepViewModel(step),
            _ => new EditGeneralStepViewModel(step)
        };
    }
}
