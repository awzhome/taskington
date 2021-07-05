using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    public static class PlanStepExtensions
    {
        public static EditStepViewModelBase ToViewModel(this PlanStep step, EditPlanViewModel parentModel) => step switch
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
