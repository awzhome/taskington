using PPBackup.Base.Model;

namespace PPBackup.Base.Executors
{
    public class StepExecutionStatus : NotifiableObject
    {
        private string? stateText;

        public string? StateText
        {
            get => stateText;
            set
            {
                stateText = value;
                NotifyPropertyChange();
            }
        }

        private int progress = 0;
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                NotifyPropertyChange();
            }
        }
    }
}
