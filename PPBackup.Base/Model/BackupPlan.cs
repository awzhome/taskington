using System.Collections.ObjectModel;

namespace PPBackup.Base.Model
{
    public class BackupPlan : Model
    {
        public BackupPlan(string type)
        {
            this.type = type;
        }

        private string? name;
        public string? Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChange();
            }
        }

        private string type;
        public string RunType
        {
            get => type;
            set
            {
                type = value;
                NotifyPropertyChange();
            }
        }

        public ObservableCollection<BackupStep> Steps { get; } = new ObservableCollection<BackupStep>();
    }
}
