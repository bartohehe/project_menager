using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProjectManager.Helpers;
using ProjectManager.Models;

namespace ProjectManager.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ProjectStatus status)
        {
            return status switch
            {
                ProjectStatus.Planned => Theme.BrushStatusPlanned,
                ProjectStatus.InProgress => Theme.BrushStatusInProgress,
                ProjectStatus.Completed => Theme.BrushStatusCompleted,
                _ => Theme.BrushTextMuted
            };
        }

        if (value is TimelineEntryType entryType)
        {
            return entryType switch
            {
                TimelineEntryType.Start => Theme.BrushStatusCompleted,
                TimelineEntryType.Completion => Theme.BrushStatusCompleted,
                TimelineEntryType.Milestone => Theme.BrushAccentPurple,
                TimelineEntryType.Photo => Theme.BrushAccentBlue,
                TimelineEntryType.PostCompletion => Theme.BrushAccent,
                _ => Theme.BrushStatusInProgress
            };
        }

        return Theme.BrushTextMuted;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class StatusToTextConverter : IValueConverter
{
    public static IReadOnlyList<ProjectStatus> AllStatuses { get; } =
        Enum.GetValues<ProjectStatus>().ToArray();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ProjectStatus status)
        {
            return status switch
            {
                ProjectStatus.Planned    => "Planowany",
                ProjectStatus.InProgress => "W trakcie",
                ProjectStatus.Completed  => "Ukończony",
                _ => "Nieznany"
            };
        }
        return "Nieznany";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
