using Avalonia;

namespace Taskington.Gui.Controls;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Taskington.Gui.Extension;

public partial class FontAwesomeIcon : UserControl
{
    private TextBlock? iconTextBlock;

    public FontAwesomeIcon()
    {
        InitializeComponent();
        UpdateGlyph();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        iconTextBlock = this.FindControl<TextBlock>("IconTextBlock");
    }

    public static readonly DirectProperty<FontAwesomeIcon, FontAwesomeIconKind?> IconProperty =
        AvaloniaProperty.RegisterDirect<FontAwesomeIcon, FontAwesomeIconKind?>(
            nameof(Icon),
            o => o.Icon,
            (o, v) => o.Icon = v);

    private FontAwesomeIconKind? icon;
    public FontAwesomeIconKind? Icon
    {
        get => icon;
        set
        {
            SetAndRaise(IconProperty, ref icon, value);
            UpdateGlyph();
        }
    }

    private void UpdateGlyph()
    {
        if (iconTextBlock != null)
        {
            iconTextBlock.Text = Icon?.ToGlyph() ?? string.Empty;
        }
    }
}
