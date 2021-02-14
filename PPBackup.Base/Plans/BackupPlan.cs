using PPBackup.Base.Steps;
using System.Collections.Generic;
using System.Linq;

namespace PPBackup.Base.Plans
{
    public class BackupPlan : Model.Model
    {
        public const string OnSelectionRunType = "selection";

        public BackupPlan(string type)
        {
            RunType = type;
        }

        public string? Name { get; set; }

        public string RunType { get; set; }

        public IEnumerable<BackupStep> Steps { get; set; } = Enumerable.Empty<BackupStep>();
    }
}
