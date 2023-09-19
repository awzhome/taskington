using System.Collections.ObjectModel;
using Taskington.Base.SystemOperations;

namespace Taskington.Gui.Extension;

public enum PathFragmentColor
{
    BrightYellow,
    Yellow,
    Blue
}

public class PathFragment
{
    public PathFragment(string text, bool isPlaceholder)
    {
        Text = text;
        IsPlaceholder = isPlaceholder;

        if (isPlaceholder)
        {
            switch (text)
            {
                case { } when text.StartsWith("drive:"):
                    Color = PathFragmentColor.Blue;
                    Icon = "fas fa-hdd";
                    Text = text[6..];
                    break;

                default:
                    Color = PathFragmentColor.Yellow;
                    Icon = "fas fa-folder";
                    break;
            };
        }
    }

    public string Text { get; }
    public bool IsPlaceholder { get; }
    public string? ExpandedText { get; set; }
    public string? Icon { get; }
    public PathFragmentColor Color { get; } = PathFragmentColor.BrightYellow;
}

public class StepPathFragment : StepCaptionFragment
{
    private readonly Placeholders placeholders;

    public StepPathFragment(Placeholders placeholders)
    {
        this.placeholders = placeholders;
    }

    public override string? Text
    {
        get => base.Text;
        set
        {
            base.Text = value;

            PathFragments.Clear();
            if (value != null)
            {
                ExtractFragments(value);
            }
        }
    }

    public ObservableCollection<PathFragment> PathFragments { get; } = new();

    private void ExtractFragments(string input)
    {
        int pos = 0;
        while (pos < input.Length)
        {
            int placeholderStart = input.IndexOf("${", pos);
            if (placeholderStart == -1)
            {
                PathFragments.Add(new PathFragment(input[pos..], false));
                break;
            }
            else
            {
                int placeholderEnd = input.IndexOf('}', placeholderStart);
                if (placeholderEnd == -1)
                {

                    PathFragments.Add(new PathFragment(input[placeholderStart..], true));
                    break;
                }
                else
                {
                    string placeholder = input.Substring(placeholderStart + 2, placeholderEnd - placeholderStart - 2);
                    PathFragments.Add(new PathFragment(placeholder, true)
                    {
                        ExpandedText = placeholders[placeholder]
                    });
                    pos = placeholderEnd + 1;
                }
            }
        }
    }
}
