namespace Taskington.Gui.Extension;

using System.Collections.Generic;
using Taskington.Base.Model;
using Taskington.Base.Steps;

public class StepCaptionFragment : NotifiableObject
{
    private string? text;
    public virtual string? Text
    {
        get => text;
        set => SetAndNotify(ref text, value);
    }
}

public interface IEditStepViewModel
{
    public string? Icon { get; set; }

    public IEnumerable<StepCaptionFragment>? CaptionFragments { get; set; }

    public string? StepType { get; set; }

    public string? SubType { get; set; }

    PlanStep ConvertToStep();
}
