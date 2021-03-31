using PPBackup.Base.Steps;
using ReactiveUI;
using System.Text;

namespace PPBackup.Gui.ViewModels
{
    public class EditBackupStepViewModel : ViewModelBase
    {
        public EditBackupStepViewModel(BackupStep step)
        {
            InitializeFromBasicModel(step);
        }

        private string? caption;
        public string? Caption
        {
            get => caption;
            set => this.RaiseAndSetIfChanged(ref caption, value);
        }

        private void InitializeFromBasicModel(BackupStep baseModel)
        {
            StringBuilder sb = new();
            sb.Append($"{baseModel.StepType} {baseModel.DefaultProperty} ");
            foreach (var property in baseModel.Properties)
            {
                sb.Append($"{property.Key} {property.Value} ");
            }
            Caption = sb.ToString();
        }
    }
}
