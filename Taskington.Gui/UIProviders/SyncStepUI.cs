using System.Collections.Generic;
using Taskington.Base.Steps;
using Taskington.Gui.Extension;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.UIProviders
{
    class SyncStepUI : IStepTypeUI
    {
        public string StepType => "sync";

        public IEditStepViewModel CreateEditViewModel(PlanStep step, IEditPlanViewModel parentModel) =>
            new EditSyncStepViewModel(step)
            {
                OpenFolderDialogInteraction = parentModel.OpenFolderDialog,
                OpenFileDialogInteraction = parentModel.OpenFileDialog
            };

        public IEnumerable<NewStepTemplate> GetNewStepTemplates()
        {
            yield return new()
            {
                Caption = "Synchronize file",
                Icon = "fas fa-copy",
                Creator = () => new PlanStep("sync")
                {
                    DefaultProperty = "file"
                }
            };
            yield return new()
            {
                Caption = "Synchronize directory",
                Icon = "fas fa-folder-open",
                Creator = () => new PlanStep("sync")
                {
                    DefaultProperty = "dir"
                }
            };
            yield return new()
            {
                Caption = "Synchronize sub-directories",
                Icon = "fas fa-sitemap",
                Creator = () => new PlanStep("sync")
                {
                    DefaultProperty = "sub-dirs"
                }
            };
        }
    }
}
