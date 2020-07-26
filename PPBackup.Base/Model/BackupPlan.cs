using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        public List<BackupStep> Steps { get; } = new List<BackupStep>();
    }
}
