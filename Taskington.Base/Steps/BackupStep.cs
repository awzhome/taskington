using System.Collections.Generic;

namespace PPBackup.Base.Steps
{
    public class BackupStep : Model.Model
    {
        public BackupStep(string type) : base()
        {
            StepType = type;
        }

        public BackupStep(string type, IEnumerable<KeyValuePair<string, string>> initialProperties)
            : base(initialProperties)
        {
            StepType = type;
        }

        public string? DefaultProperty { get; set; }

        public string StepType { get; }
    }
}
