using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    class EditGeneralStepViewModel : EditStepViewModelBase
    {
        public EditGeneralStepViewModel(PlanStep step) : base(step)
        {
            InitializeFromBasicModel(step);
        }

        private string? readableProperties;
        public string? ReadableProperties
        {
            get => readableProperties;
            set => this.RaiseAndSetIfChanged(ref readableProperties, value);
        }

        private void InitializeFromBasicModel(PlanStep baseModel)
        {
            StringBuilder readablePropertiesBuilder = new();
            readablePropertiesBuilder.Append($"{baseModel.StepType} {baseModel.DefaultProperty}{Environment.NewLine}");
            StringBuilder captionBuilder = new();
            captionBuilder.Append($"{baseModel.StepType} {baseModel.DefaultProperty} ");

            foreach (var property in baseModel.Properties)
            {
                readablePropertiesBuilder.Append($"    {property.Key} {property.Value}{Environment.NewLine}");
                captionBuilder.Append($"{property.Key} {property.Value} ");
            }
            ReadableProperties = readablePropertiesBuilder.ToString();
            Caption = captionBuilder.ToString();
        }
    }
}
