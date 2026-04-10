using System.Windows;
using System.Windows.Controls;
using ProjectManager.Models;

namespace ProjectManager.Views.Dialogs;

public partial class AddTimelineEntryDialog : Window
{
    public string EntryTitle { get; private set; } = string.Empty;
    public string EntryDescription { get; private set; } = string.Empty;
    public TimelineEntryType EntryType { get; private set; } = TimelineEntryType.Update;

    public AddTimelineEntryDialog()
    {
        InitializeComponent();
    }

    private void OnConfirm(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            TitleBox.Focus();
            return;
        }

        EntryTitle = TitleBox.Text.Trim();
        EntryDescription = DescriptionBox.Text?.Trim() ?? string.Empty;

        if (TypeCombo.SelectedItem is ComboBoxItem item && item.Tag is TimelineEntryType type)
            EntryType = type;

        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
