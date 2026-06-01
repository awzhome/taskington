using Avalonia;

namespace Taskington.Gui.Controls;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

public partial class FontAwesomeIcon : UserControl
{
    public FontAwesomeIcon()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static readonly DirectProperty<FontAwesomeIcon, string?> IconProperty =
        AvaloniaProperty.RegisterDirect<FontAwesomeIcon, string?>(
            nameof(Icon),
            o => o.Icon,
            (o, v) => o.Icon = v);

    public string? Icon
    {
        get;
        set => SetAndRaise(IconProperty, ref field, value);
    }
}
