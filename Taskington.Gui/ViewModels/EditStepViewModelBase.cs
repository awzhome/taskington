using ReactiveUI;
using System.Linq;
using System.Text;
using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    public class EditStepViewModelBase : ViewModelBase
    {
        private readonly BackupStep step;

        public EditStepViewModelBase(BackupStep step)
        {
            this.step = step;
            InitializeFromBasicModel();
        }

        private string? caption;
        public string? Caption
        {
            get => caption;
            set => this.RaiseAndSetIfChanged(ref caption, value);
        }

        private string? stepType;
        public string? StepType
        {
            get => stepType;
            set => this.RaiseAndSetIfChanged(ref stepType, value);
        }

        private string? subType;
        public string? SubType
        {
            get => subType;
            set => this.RaiseAndSetIfChanged(ref subType, value);
        }

        private void InitializeFromBasicModel()
        {
            stepType = step.StepType;
            SubType = step.DefaultProperty;

            StringBuilder sb = new();
            sb.Append($"{step.StepType} {step.DefaultProperty} ");
            foreach (var property in step.Properties)
            {
                sb.Append($"{property.Key} {property.Value} ");
            }
            Caption = sb.ToString();
        }

        public virtual BackupStep ConvertToStep()
        {
            return new(stepType!, step.Properties)
            {
                DefaultProperty = subType
            };
        }
    }
}
