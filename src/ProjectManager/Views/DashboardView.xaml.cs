using System.Windows.Controls;
using ProjectManager.ViewModels;

namespace ProjectManager.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is DashboardViewModel vm)
            vm.LoadDataCommand.Execute(null);
    }
}
