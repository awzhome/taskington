namespace PPBackup.Base.Steps
{
    public class BackupStep : Model.Model
    {
        public BackupStep(string type)
        {
            StepType = type;
        }

        public string? DefaultProperty { get; set; }

        public string StepType { get; }
    }
}
