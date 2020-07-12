namespace PPBackup.Base.Model
{
    public class BackupStep : Model
    {
        public BackupStep(string type) : base(type)
        {
        }

        public string? DefaultProperty { get; set; }
    }
}
