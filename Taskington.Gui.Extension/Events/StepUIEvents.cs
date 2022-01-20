using System.Collections.Generic;
using Taskington.Base.Steps;
using Taskington.Base.TinyBus;

namespace Taskington.Gui.Extension.Events
{
    public static class StepUIEvents
    {
        public static RequestEvent<PlanStep, IEditPlanViewModel, IEditStepViewModel> NewEditViewModel { get; } = new();
        public static RequestEvent<IEnumerable<NewStepTemplate>> NewStepTemplates { get; } = new();
    }
}
