using System.Windows.Controls;
using ProjectManager.ViewModels;

namespace ProjectManager.Views;

public partial class ProjectListView : UserControl
{
    public ProjectListView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ProjectListViewModel vm)
            vm.LoadProjectsCommand.Execute(null);
    }
}
