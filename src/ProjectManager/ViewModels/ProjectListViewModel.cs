using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectManager.Models;
using ProjectManager.Services;

namespace ProjectManager.ViewModels;

public partial class ProjectListViewModel : ObservableObject
{
    private readonly IProjectRepository _projectRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly INavigationService _navigation;
    private readonly IMongoDbService _mongoDb;

    [ObservableProperty]
    private string _filterName = string.Empty;

    [ObservableProperty]
    private string? _filterCategory;

    [ObservableProperty]
    private DateTime? _filterStartDate;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    public ObservableCollection<Project> Projects { get; } = [];
    public ObservableCollection<string> Categories { get; } = [];

    public ProjectListViewModel(
        IProjectRepository projectRepo,
        ICategoryRepository categoryRepo,
        INavigationService navigation,
        IMongoDbService mongoDb)
    {
        _projectRepo = projectRepo;
        _categoryRepo = categoryRepo;
        _navigation = navigation;
        _mongoDb = mongoDb;
    }

    [RelayCommand]
    private async Task LoadProjectsAsync()
    {
        if (!_mongoDb.IsConnected) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var categories = await _categoryRepo.GetAllAsync();
            Categories.Clear();
            Categories.Add("Wszystkie");
            foreach (var cat in categories)
                Categories.Add(cat.Name);

            await ApplyFiltersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd ładowania: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ApplyFiltersAsync()
    {
        if (!_mongoDb.IsConnected) return;

        try
        {
            var filter = new ProjectFilter
            {
                Name = string.IsNullOrWhiteSpace(FilterName) ? null : FilterName,
                Category = FilterCategory is "Wszystkie" or null ? null : FilterCategory,
                StartDateFrom = FilterStartDate
            };

            var projects = await _projectRepo.GetAllAsync(filter);
            Projects.Clear();
            foreach (var project in projects)
                Projects.Add(project);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd filtrowania: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearFilters()
    {
        FilterName = string.Empty;
        FilterCategory = null;
        FilterStartDate = null;
    }

    [RelayCommand]
    private void OpenProject(Project project)
    {
        _navigation.NavigateTo<ProjectDetailViewModel>(project.Id);
    }

    [RelayCommand]
    private void AddNewProject()
    {
        _navigation.NavigateTo<NewProjectViewModel>();
    }
}
