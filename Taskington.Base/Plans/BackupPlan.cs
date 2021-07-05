using System.Collections.Generic;
using System.Linq;
using Taskington.Base.Steps;

namespace Taskington.Base.Plans
{
    public class BackupPlan : Model.Model
    {
        public const string OnSelectionRunType = "selection";

        public BackupPlan(string type) : base()
        {
            RunType = type;
        }

        public BackupPlan(string type, IEnumerable<KeyValuePair<string, string>> initialProperties)
            : base(initialProperties)
        {
            RunType = type;
        }

        public string? Name { get; set; }

        public string RunType { get; set; }

        public IEnumerable<BackupStep> Steps { get; set; } = Enumerable.Empty<BackupStep>();
    }
}
