using System.Collections.Generic;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Gui.Extension.Messages
{
    public record NewEditViewModelMessage(
        PlanStep Step,
        IEditPlanViewModel ParentModel,
        Placeholders Placeholders) : RequestMessage<NewEditViewModelMessage, IEditStepViewModel>;
    public record NewStepTemplatesMessage : RequestMessage<IEnumerable<NewStepTemplate>>;
}
