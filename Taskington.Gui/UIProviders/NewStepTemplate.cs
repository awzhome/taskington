using System;
using Taskington.Base.Steps;

namespace Taskington.Gui.UIProviders
{
    public class NewStepTemplate
    {
        public string? Caption { get; set; }
        public Func<PlanStep>? Creator { get; set; }
    }
}
