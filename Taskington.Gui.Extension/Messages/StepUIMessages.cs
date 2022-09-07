using System.Collections.Generic;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Gui.Extension.Messages
{
    public static class StepUIMessages
    {
        public static RequestMessageEndPoint<PlanStep, IEditPlanViewModel, Placeholders, IEditStepViewModel> NewEditViewModel { get; } = new();
        public static RequestMessageEndPoint<IEnumerable<NewStepTemplate>> NewStepTemplates { get; } = new();
    }
}
