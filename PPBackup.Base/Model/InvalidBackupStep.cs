namespace PPBackup.Base.Model
{
    public class InvalidBackupStep : BackupStep
    {
        public InvalidBackupStep(string errorMessage) : base("invalid")
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
    }
}
