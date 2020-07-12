using PPBackup.Base.Model;

namespace PPBackup.Base.Executors
{
    public class PlanExecutionStatus : NotifiableObject
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

        private bool canExecute = true;
        public bool CanExecute
        {
            get => canExecute;
            set
            {
                canExecute = value;
                NotifyPropertyChange();
            }
        }

        private bool isPaused = false;
        public bool IsPaused
        {
            get => isPaused;
            set
            {
                isPaused = value;
                NotifyPropertyChange();
            }
        }

        private bool isRunning = false;
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
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

        private bool hasErrors = false;
        public bool HasErrors
        {
            get => hasErrors;
            set
            {
                hasErrors = value;
                NotifyPropertyChange();
            }
        }
    }
}
