using PPBackup.Base.Steps;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPBackup.Gui.ViewModels
{
    public class EditGeneralStepViewModel : EditStepViewModelBase
    {
        public EditGeneralStepViewModel(BackupStep step) : base(step)
        {
            InitializeFromBasicModel(step);
        }

        private string? readableProperties;
        public string? ReadableProperties
        {
            get => readableProperties;
            set => this.RaiseAndSetIfChanged(ref readableProperties, value);
        }

        private void InitializeFromBasicModel(BackupStep baseModel)
        {
            StringBuilder sb = new();
            sb.Append($"{baseModel.StepType} {baseModel.DefaultProperty}{Environment.NewLine}");
            foreach (var property in baseModel.Properties)
            {
                sb.Append($"    {property.Key} {property.Value}{Environment.NewLine}");
            }
            ReadableProperties = sb.ToString();
        }
    }
}
