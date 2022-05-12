using System.Collections.Generic;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;

namespace Taskington.Gui.Extension.Messages
{
    public static class StepUIMessages
    {
        public static RequestMessage<PlanStep, IEditPlanViewModel, Placeholders, IEditStepViewModel> NewEditViewModel { get; } = new();
        public static RequestMessage<IEnumerable<NewStepTemplate>> NewStepTemplates { get; } = new();
    }
}
