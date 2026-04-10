using System.Windows;

namespace ProjectManager.Views.Dialogs;

public partial class AddSectionDialog : Window
{
    public string SectionTitle { get; private set; } = string.Empty;
    public string SectionContent { get; private set; } = string.Empty;

    public AddSectionDialog()
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

        SectionTitle = TitleBox.Text.Trim();
        SectionContent = ContentBox.Text?.Trim() ?? string.Empty;
        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
