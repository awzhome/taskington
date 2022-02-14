namespace Taskington.Gui.Extension;

using System.Collections.ObjectModel;

public class PathFragment
{
    public PathFragment(string text, bool isPlaceholder)
    {
        Text = text;
        IsPlaceholder = isPlaceholder;
    }

    public string Text { get; }
    public bool IsPlaceholder { get; }
    public string? ExpandedText { get; init; }
}

public class StepPathFragment : StepCaptionFragment
{
    public override string? Text
    {
        get => base.Text;
        set
        {
            base.Text = value;

            PathFragments.Clear();
            if (value != null)
            {
                PathFragments.Add(new(value, false));
            }
        }
    }

    public ObservableCollection<PathFragment> PathFragments { get; } = new();
}
