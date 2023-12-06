using System.Collections.Generic;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;

namespace Taskington.Gui.Extension;

public interface IStepUI
{
    IEditStepViewModel CreateEditViewModel(PlanStep step, IEditPlanViewModel parentModel, Placeholders placeholders);
    IEnumerable<NewStepTemplate> GetNewStepTemplates();
}