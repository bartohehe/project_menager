using System.Windows.Controls;
using ProjectManager.ViewModels;

namespace ProjectManager.Views;

public partial class NewProjectView : UserControl
{
    public NewProjectView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is NewProjectViewModel vm)
            vm.LoadCategoriesCommand.Execute(null);
    }
}
