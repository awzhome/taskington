using System.Collections.Generic;

namespace Taskington.Base.Steps
{
    public class PlanStep : Model.Model
    {
        public PlanStep(string type) : base()
        {
            StepType = type;
        }

        public PlanStep(string type, IEnumerable<KeyValuePair<string, string>> initialProperties)
            : base(initialProperties)
        {
            StepType = type;
        }

        public string? DefaultProperty { get; set; }

        public string StepType { get; }
    }
}
