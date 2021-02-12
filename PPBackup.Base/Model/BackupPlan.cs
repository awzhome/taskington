using System.Collections.Generic;
using System.Linq;

namespace PPBackup.Base.Model
{
    public class BackupPlan : Model
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
