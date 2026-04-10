using System.Windows;

namespace ProjectManager.Views.Dialogs;

public partial class ConfirmDialog : Window
{
    public ConfirmDialog(string title, string message, bool isDanger = false)
    {
        InitializeComponent();
        TitleText.Text = title;
        MessageText.Text = message;

        if (isDanger)
        {
            ConfirmBtn.Style = (Style)FindResource("FlatButton");
            ConfirmBtn.Foreground = (System.Windows.Media.Brush)FindResource("BrushDanger");
            ConfirmBtn.Content = "Usuń";
        }
    }

    private void OnConfirm(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
