using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectManager.Helpers;
using ProjectManager.Models;
using ProjectManager.Services;

namespace ProjectManager.ViewModels;

public partial class NewProjectViewModel : ObservableValidator
{
    private readonly IProjectRepository _projectRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly INavigationService _navigation;
    private readonly IMongoDbService _mongoDb;

    [ObservableProperty]
    [Required(ErrorMessage = "Nazwa projektu jest wymagana.")]
    [MaxLength(200, ErrorMessage = "Nazwa nie może przekraczać 200 znaków.")]
    [NotifyDataErrorInfo]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = string.Empty;

    [ObservableProperty]
    private string _newCategoryName = string.Empty;

    [ObservableProperty]
    [MaxLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków.")]
    [NotifyDataErrorInfo]
    private string _shortDescription = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _successMessage;

    public ObservableCollection<string> Categories { get; } = [];

    public NewProjectViewModel(
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
    private async Task LoadCategoriesAsync()
    {
        if (!_mongoDb.IsConnected) return;

        try
        {
            var categories = await _categoryRepo.GetAllAsync();
            Categories.Clear();
            foreach (var cat in categories)
                Categories.Add(cat.Name);
        }
        catch { /* Categories will be empty */ }
    }

    [RelayCommand]
    private async Task CreateProjectAsync()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        var nameValidation = InputValidator.ValidateProjectName(Name);
        if (!nameValidation.IsValid)
        {
            ErrorMessage = nameValidation.Error;
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            var category = SelectedCategory;

            if (!string.IsNullOrWhiteSpace(NewCategoryName))
            {
                category = InputValidator.SanitizeString(NewCategoryName);
                await _categoryRepo.CreateAsync(new Category { Name = category });
            }

            var project = new Project
            {
                Name = InputValidator.SanitizeString(Name),
                Category = category,
                ShortDescription = InputValidator.SanitizeString(ShortDescription),
                Status = ProjectStatus.Planned
            };

            await _projectRepo.CreateAsync(project);
            _navigation.NavigateTo<ProjectListViewModel>();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd tworzenia projektu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
