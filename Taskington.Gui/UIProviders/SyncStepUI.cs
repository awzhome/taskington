using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskington.Base.Steps;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.UIProviders
{
    class SyncStepUI : IStepTypeUI
    {
        public string StepType => "sync";

        public EditStepViewModelBase CreateEditViewModel(PlanStep step, EditPlanViewModel parentModel) =>
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
                Creator = () => new PlanStep("sync")
                {
                    DefaultProperty = "file"
                }
            };
            yield return new()
            {
                Caption = "Synchronize directory",
                Creator = () => new PlanStep("sync")
                {
                    DefaultProperty = "dir"
                }
            };
            yield return new()
            {
                Caption = "Synchronize sub-directories",
                Creator = () => new PlanStep("sync")
                {
                    DefaultProperty = "sub-dirs"
                }
            };
        }
    }
}
