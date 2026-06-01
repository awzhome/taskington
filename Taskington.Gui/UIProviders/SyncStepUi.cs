using System.Collections.Generic;
using Taskington.Base.Extension;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Gui.Extension;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.UIProviders;

class SyncStepUi : IStepUi
{
    public SyncStepUi(IKeyedRegistry<IStepUi> stepUIs)
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
        yield return new NewStepTemplate
        {
            Caption = "Synchronize file",
            Icon = FontAwesomeIconKind.File,
            Creator = () => new PlanStep("sync")
            {
                DefaultProperty = "file"
            }
        };
        yield return new NewStepTemplate
        {
            Caption = "Synchronize directory",
            Icon = FontAwesomeIconKind.FolderOpen,
            Creator = () => new PlanStep("sync")
            {
                DefaultProperty = "dir"
            }
        };
        yield return new NewStepTemplate
        {
            Caption = "Synchronize sub-directories",
            Icon = FontAwesomeIconKind.Sitemap,
            Creator = () => new PlanStep("sync")
            {
                DefaultProperty = "sub-dirs"
            }
        };
    }
}
