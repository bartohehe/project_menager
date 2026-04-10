using System.Windows.Media;

namespace ProjectManager.Helpers;

public static class Theme
{
    // Backgrounds
    public static readonly Color BgDark = Color.FromRgb(13, 17, 27);
    public static readonly Color BgPanel = Color.FromRgb(20, 27, 42);
    public static readonly Color BgCard = Color.FromRgb(26, 35, 53);
    public static readonly Color BgHover = Color.FromRgb(33, 44, 66);
    public static readonly Color BorderColor = Color.FromRgb(40, 55, 80);

    // Accents
    public static readonly Color Accent = Color.FromRgb(0, 188, 140);
    public static readonly Color AccentHover = Color.FromRgb(0, 210, 160);
    public static readonly Color AccentBlue = Color.FromRgb(59, 130, 246);
    public static readonly Color AccentPurple = Color.FromRgb(139, 92, 246);

    // Text
    public static readonly Color TextPrimary = Color.FromRgb(240, 245, 255);
    public static readonly Color TextSecondary = Color.FromRgb(130, 150, 185);
    public static readonly Color TextMuted = Color.FromRgb(80, 100, 135);

    // Status
    public static readonly Color StatusPlanned = Color.FromRgb(59, 130, 246);
    public static readonly Color StatusInProgress = Color.FromRgb(251, 191, 36);
    public static readonly Color StatusCompleted = Color.FromRgb(34, 197, 94);

    // Danger
    public static readonly Color Danger = Color.FromRgb(239, 68, 68);
    public static readonly Color DangerHover = Color.FromRgb(255, 100, 100);

    // Frozen Brushes
    public static readonly SolidColorBrush BrushBgDark = Freeze(BgDark);
    public static readonly SolidColorBrush BrushBgPanel = Freeze(BgPanel);
    public static readonly SolidColorBrush BrushBgCard = Freeze(BgCard);
    public static readonly SolidColorBrush BrushBgHover = Freeze(BgHover);
    public static readonly SolidColorBrush BrushBorder = Freeze(BorderColor);

    public static readonly SolidColorBrush BrushAccent = Freeze(Accent);
    public static readonly SolidColorBrush BrushAccentHover = Freeze(AccentHover);
    public static readonly SolidColorBrush BrushAccentBlue = Freeze(AccentBlue);
    public static readonly SolidColorBrush BrushAccentPurple = Freeze(AccentPurple);

    public static readonly SolidColorBrush BrushTextPrimary = Freeze(TextPrimary);
    public static readonly SolidColorBrush BrushTextSecondary = Freeze(TextSecondary);
    public static readonly SolidColorBrush BrushTextMuted = Freeze(TextMuted);

    public static readonly SolidColorBrush BrushStatusPlanned = Freeze(StatusPlanned);
    public static readonly SolidColorBrush BrushStatusInProgress = Freeze(StatusInProgress);
    public static readonly SolidColorBrush BrushStatusCompleted = Freeze(StatusCompleted);

    public static readonly SolidColorBrush BrushDanger = Freeze(Danger);
    public static readonly SolidColorBrush BrushDangerHover = Freeze(DangerHover);

    public static readonly SolidColorBrush BrushTransparent = Freeze(Colors.Transparent);

    private static SolidColorBrush Freeze(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}
