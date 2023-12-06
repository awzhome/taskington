using System.Collections.Generic;
using Taskington.Base.Extension;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Gui.Extension;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.UIProviders;

class SyncStepUI : IStepUI
{
    public SyncStepUI(IKeyedRegistry<IStepUI> stepUIs)
    {
        stepUIs.Add("sync", this);
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
            Icon = "fas fa-file",
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
