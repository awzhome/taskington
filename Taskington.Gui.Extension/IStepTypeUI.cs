using System.Collections.Generic;
using Taskington.Base.Steps;

namespace Taskington.Gui.Extension
{
    public interface IStepTypeUI
    {
        string StepType { get; }

        IEditStepViewModel CreateEditViewModel(PlanStep step, IEditPlanViewModel parentModel);
        IEnumerable<NewStepTemplate> GetNewStepTemplates();
    }
}
