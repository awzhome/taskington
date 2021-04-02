using PPBackup.Base.Steps;

namespace PPBackup.Gui.ViewModels
{
    public static class EditStepViewModelFactory
    {
        public static EditStepViewModelBase Create(BackupStep step) => step switch
        {
            { StepType: "sync"} => new EditSyncStepViewModel(step),
            _ => new EditGeneralStepViewModel(step)
        };
    }
}
