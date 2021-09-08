using System;
using Taskington.Base.Steps;

namespace Taskington.Gui.UIProviders
{
    class NewStepTemplate
    {
        public string? Caption { get; set; }
        public Func<PlanStep>? Creator { get; set; }
    }
}
