using System.Windows;
using System.Windows.Controls;
using ProjectManager.Models;
using ProjectManager.ViewModels;

namespace ProjectManager.Views;

public partial class ProjectDetailView : UserControl
{
    public ProjectDetailView()
    {
        InitializeComponent();
    }

    private void OnAddTimelineEntry(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ProjectDetailViewModel vm) return;

        var dialog = new Dialogs.AddTimelineEntryDialog
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true)
        {
            var entry = (dialog.EntryTitle, dialog.EntryDescription, dialog.EntryType);
            vm.AddTimelineEntryCommand.Execute(entry);
        }
    }

    private void OnAddSection(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ProjectDetailViewModel vm) return;

        var dialog = new Dialogs.AddSectionDialog
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true)
        {
            vm.AddSectionCommand.Execute(dialog.SectionTitle);
        }
    }

    private void OnDeleteProject(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ProjectDetailViewModel vm) return;

        var dialog = new Dialogs.ConfirmDialog(
            "Usunięcie projektu",
            "Czy na pewno chcesz usunąć ten projekt? Wszystkie dane, zdjęcia i pliki zostaną nieodwracalnie usunięte.",
            isDanger: true)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true)
        {
            vm.DeleteProjectCommand.Execute(null);
        }
    }
}
