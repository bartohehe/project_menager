using System.Globalization;
using System.Windows.Data;

namespace ProjectManager.Converters;

public class DateFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            var format = parameter as string ?? "dd.MM.yyyy";
            return date.ToLocalTime().ToString(format, new CultureInfo("pl-PL"));
        }
        return "—";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class RelativeDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DateTime date)
            return "—";

        var local = date.ToLocalTime();
        var diff = DateTime.Now - local;

        return diff.TotalMinutes switch
        {
            < 1 => "przed chwilą",
            < 60 => $"{(int)diff.TotalMinutes} min temu",
            < 1440 => $"{(int)diff.TotalHours} godz. temu",
            < 2880 => "wczoraj",
            < 10080 => $"{(int)diff.TotalDays} dni temu",
            _ => local.ToString("dd.MM.yyyy", new CultureInfo("pl-PL"))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
