using System.Collections.Generic;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Gui.Extension;
using Taskington.Gui.Extension.Events;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.UIProviders
{
    class SyncStepUI
    {
        public const string StepType = "sync";

        public SyncStepUI()
        {
            StepUIEvents.NewEditViewModel.Subscribe(CreateEditViewModel, (step, parentModel, placeholders) => step.StepType == StepType);
            StepUIEvents.NewStepTemplates.Subscribe(GetNewStepTemplates);
        }

        public IEditStepViewModel CreateEditViewModel(PlanStep step, IEditPlanViewModel parentModel, Placeholders placeholders)
        {
            return new EditSyncStepViewModel(step, placeholders)
            {
                OpenFolderDialogInteraction = parentModel.OpenFolderDialog,
                OpenFileDialogInteraction = parentModel.OpenFileDialog
            };
        }

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
