namespace PPBackup.Base.Model
{
    public class BackupStep : Model
    {
        public BackupStep(string type)
        {
            StepType = type;
        }

        public string? DefaultProperty { get; set; }

        public string StepType { get; }
    }
}
