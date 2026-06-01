using System;

namespace Taskington.Gui.Extension;

public enum FontAwesomeIconKind
{
    Plus,
    Copy,
    ArrowUp,
    ArrowDown,
    TrashCan,
    Play,
    EllipsisVertical,
    PenToSquare,
    ArrowRotateLeft,
    CircleInfo,
    File,
    FolderOpen,
    Sitemap,
    HardDrive,
    Folder
}

public static class FontAwesomeIconKindExtensions
{
    public static string ToGlyph(this FontAwesomeIconKind icon) =>
        icon switch
        {
            FontAwesomeIconKind.Plus => "\u002B",
            FontAwesomeIconKind.Copy => "\uF0C5",
            FontAwesomeIconKind.ArrowUp => "\uF062",
            FontAwesomeIconKind.ArrowDown => "\uF063",
            FontAwesomeIconKind.TrashCan => "\uF2ED",
            FontAwesomeIconKind.Play => "\uF04B",
            FontAwesomeIconKind.EllipsisVertical => "\uF142",
            FontAwesomeIconKind.PenToSquare => "\uF304",
            FontAwesomeIconKind.ArrowRotateLeft => "\uF2EA",
            FontAwesomeIconKind.CircleInfo => "\uF05A",
            FontAwesomeIconKind.File => "\uF15B",
            FontAwesomeIconKind.FolderOpen => "\uF07C",
            FontAwesomeIconKind.Sitemap => "\uF0E8",
            FontAwesomeIconKind.HardDrive => "\uF0A0",
            FontAwesomeIconKind.Folder => "\uF07B",
            _ => throw new ArgumentOutOfRangeException(nameof(icon), icon, null)
        };
}
