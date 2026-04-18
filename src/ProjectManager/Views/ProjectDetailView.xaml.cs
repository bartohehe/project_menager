using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ProjectManager.Models;
using ProjectManager.ViewModels;

namespace ProjectManager.Views;

public partial class ProjectDetailView : UserControl
{
    // ── Drag-and-drop state ──────────────────────────────────────────────
    private SectionCard? _dragCard;
    private Point        _dragStartPoint;
    private bool         _isDragging;

    private const string DragFormat = "SectionCard";

    public ProjectDetailView()
    {
        InitializeComponent();
    }

    // ── Timeline / section dialog handlers ──────────────────────────────

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

    // ── Drag-and-drop card rearranging ───────────────────────────────────

    /// <summary>
    /// Called on the drag-overlay's PreviewMouseLeftButtonDown.
    /// Captures the card and starting mouse position.
    /// </summary>
    private void Card_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not ProjectDetailViewModel { IsLayoutEditing: true }) return;
        if (sender is not FrameworkElement el) return;

        _dragCard       = el.DataContext as SectionCard;
        _dragStartPoint = e.GetPosition(this);
        _isDragging     = false;
    }

    /// <summary>
    /// Called on the drag-overlay's MouseMove.
    /// Initiates DragDrop once cursor moves far enough from start point.
    /// </summary>
    private void Card_MouseMove(object sender, MouseEventArgs e)
    {
        if (_dragCard is null || e.LeftButton != MouseButtonState.Pressed) return;
        if (_isDragging) return;

        var pos  = e.GetPosition(this);
        var diff = _dragStartPoint - pos;

        if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance) return;

        _isDragging = true;

        var data = new DataObject(DragFormat, _dragCard);

        if (sender is FrameworkElement el)
            DragDrop.DoDragDrop(el, data, DragDropEffects.Move);

        _isDragging = false;
        _dragCard   = null;
    }

    /// <summary>
    /// Highlights the card under the cursor during a drag operation.
    /// </summary>
    private void Card_DragOver(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DragFormat))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        e.Effects = DragDropEffects.Move;
        e.Handled = true;

        if (sender is Border border)
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xBC, 0x8C)); // Accent
    }

    /// <summary>
    /// Removes the drop-target highlight when the cursor leaves a card.
    /// </summary>
    private void Card_DragLeave(object sender, DragEventArgs e)
    {
        if (sender is Border border)
            border.ClearValue(Border.BorderBrushProperty);
    }

    /// <summary>
    /// Performs the actual move via the ViewModel when a card is dropped.
    /// </summary>
    private void Card_Drop(object sender, DragEventArgs e)
    {
        if (sender is Border border)
            border.ClearValue(Border.BorderBrushProperty);

        if (!e.Data.GetDataPresent(DragFormat)) return;
        if (DataContext is not ProjectDetailViewModel vm) return;

        var sourceCard = e.Data.GetData(DragFormat) as SectionCard;
        var targetCard = (sender as FrameworkElement)?.DataContext as SectionCard;

        if (sourceCard is null || targetCard is null || sourceCard == targetCard) return;

        vm.MoveCard(sourceCard, targetCard);
        e.Handled = true;
    }
}
