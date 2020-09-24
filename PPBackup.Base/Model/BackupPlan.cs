using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PPBackup.Base.Model
{
    public class BackupPlan : Model
    {
        public BackupPlan(string type)
        {
            RunType = type;
        }

        public string? Name { get; set; }

        public string RunType { get; set; }

        public IEnumerable<BackupStep> Steps { get; set; } = Enumerable.Empty<BackupStep>();
    }
}
