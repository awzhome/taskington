namespace PPBackup.Base.Model
{
    public class BackupStep : Model
    {
        public BackupStep(string type)
        {
            this.type = type;
        }

        public string? DefaultProperty { get; set; }

        private string type;
        public string StepType
        {
            get => type;
            set
            {
                type = value;
                NotifyPropertyChange();
            }
        }
    }
}
