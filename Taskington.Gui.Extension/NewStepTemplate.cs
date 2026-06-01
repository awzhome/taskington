using System;
using Taskington.Base.Steps;
namespace Taskington.Gui.Extension;

public class NewStepTemplate
{
    public FontAwesomeIconKind? Icon { get; set; }
    public string? Caption { get; set; }
    public Func<PlanStep>? Creator { get; set; }
}