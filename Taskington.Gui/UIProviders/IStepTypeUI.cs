using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskington.Base.Steps;
using Taskington.Gui.ViewModels;

namespace Taskington.Gui.UIProviders
{
    interface IStepTypeUI
    {
        string StepType { get; }

        EditStepViewModelBase CreateEditViewModel(PlanStep step, EditPlanViewModel parentModel);
        IEnumerable<NewStepTemplate> GetNewStepTemplates();
    }
}
