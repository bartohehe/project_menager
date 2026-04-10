using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectManager.Models;
using ProjectManager.Services;

namespace ProjectManager.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IProjectRepository _projectRepo;
    private readonly INavigationService _navigation;
    private readonly IMongoDbService _mongoDb;

    [ObservableProperty]
    private int _completedCount;

    [ObservableProperty]
    private int _inProgressCount;

    [ObservableProperty]
    private int _plannedCount;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    public ObservableCollection<Project> RecentProjects { get; } = [];

    public DashboardViewModel(IProjectRepository projectRepo, INavigationService navigation, IMongoDbService mongoDb)
    {
        _projectRepo = projectRepo;
        _navigation = navigation;
        _mongoDb = mongoDb;
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (!_mongoDb.IsConnected)
        {
            ErrorMessage = "Brak połączenia z bazą danych. Przejdź do Ustawień.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var stats = await _projectRepo.GetDashboardStatsAsync();
            CompletedCount = stats.CompletedCount;
            InProgressCount = stats.InProgressCount;
            PlannedCount = stats.PlannedCount;

            var recent = await _projectRepo.GetRecentAsync(8);
            RecentProjects.Clear();
            foreach (var project in recent)
                RecentProjects.Add(project);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd ładowania danych: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenProject(Project project)
    {
        _navigation.NavigateTo<ProjectDetailViewModel>(project.Id);
    }
}
