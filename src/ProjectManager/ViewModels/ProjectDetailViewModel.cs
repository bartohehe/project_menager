using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ProjectManager.Helpers;
using ProjectManager.Models;
using ProjectManager.Services;

namespace ProjectManager.ViewModels;

public partial class ProjectDetailViewModel : ObservableObject, IParameterReceiver
{
    private readonly IProjectRepository _projectRepo;
    private readonly IFileStorageService _fileStorage;
    private readonly INavigationService _navigation;

    private string _projectId = string.Empty;

    [ObservableProperty]
    private Project? _project;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    // Editable fields
    [ObservableProperty]
    private string _editName = string.Empty;

    [ObservableProperty]
    private string _editDescription = string.Empty;

    [ObservableProperty]
    private string _editPrice = string.Empty;

    [ObservableProperty]
    private DateTime? _editStartDate;

    [ObservableProperty]
    private DateTime? _editEndDate;

    [ObservableProperty]
    private ProjectStatus _editStatus;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGeneralTab))]
    [NotifyPropertyChangedFor(nameof(IsTimelineTab))]
    private int _activeTab = 0;

    public bool IsGeneralTab  => ActiveTab == 0;
    public bool IsTimelineTab => ActiveTab == 1;

    [ObservableProperty]
    private bool _isLayoutEditing;

    public ObservableCollection<SectionCard>    SectionCards   { get; } = [];
    public ObservableCollection<TimelineEntry>  TimelineEntries { get; } = [];
    public ObservableCollection<ProjectAttachment> Attachments { get; } = [];
    public ObservableCollection<ProjectSection> CustomSections { get; } = [];
    public ObservableCollection<byte[]> GalleryImages { get; } = [];

    public ProjectDetailViewModel(
        IProjectRepository projectRepo,
        IFileStorageService fileStorage,
        INavigationService navigation)
    {
        _projectRepo = projectRepo;
        _fileStorage = fileStorage;
        _navigation = navigation;
    }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is string projectId)
        {
            _projectId = projectId;
            LoadProjectCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task LoadProjectAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var project = await _projectRepo.GetByIdAsync(_projectId);
            if (project is null)
            {
                ErrorMessage = "Projekt nie został znaleziony.";
                return;
            }

            Project = project;
            PopulateEditFields(project);

            TimelineEntries.Clear();
            foreach (var entry in project.TimelineEntries.OrderBy(e => e.Date))
                TimelineEntries.Add(entry);

            Attachments.Clear();
            foreach (var att in project.Attachments)
                Attachments.Add(att);

            CustomSections.Clear();
            foreach (var section in project.CustomSections.OrderBy(s => s.Order))
                CustomSections.Add(section);

            await LoadGalleryImagesAsync(project);
            BuildSectionCards();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd ładowania projektu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BuildSectionCards()
    {
        if (Project is null) return;

        SectionCards.Clear();

        var defaultIds = new List<string> { "desc", "photos", "attachments" }
            .Concat(Project.CustomSections.OrderBy(s => s.Order).Select(s => s.Id))
            .ToList();

        // Merge saved order with any missing cards
        var order = Project.CardOrder.Count > 0
            ? Project.CardOrder.Union(defaultIds).ToList()
            : defaultIds;

        foreach (var id in order)
        {
            switch (id)
            {
                case "desc":
                    SectionCards.Add(new SectionCard { Type = SectionCardType.Description, Id = "desc" });
                    break;
                case "photos":
                    SectionCards.Add(new SectionCard { Type = SectionCardType.Photos, Id = "photos" });
                    break;
                case "attachments":
                    SectionCards.Add(new SectionCard { Type = SectionCardType.Attachments, Id = "attachments" });
                    break;
                default:
                    var section = Project.CustomSections.FirstOrDefault(s => s.Id == id);
                    if (section is not null)
                        SectionCards.Add(new SectionCard { Type = SectionCardType.Custom, Id = id, Section = section });
                    break;
            }
        }
    }

    /// <summary>Called from code-behind during drag-drop to reorder cards.</summary>
    public void MoveCard(SectionCard source, SectionCard target)
    {
        var si = SectionCards.IndexOf(source);
        var ti = SectionCards.IndexOf(target);
        if (si < 0 || ti < 0 || si == ti) return;
        SectionCards.Move(si, ti);
    }

    [RelayCommand]
    private async Task SaveLayoutAsync()
    {
        if (Project is null) return;
        Project.CardOrder = SectionCards.Select(c => c.Id).ToList();
        await _projectRepo.UpdateAsync(Project);
        IsLayoutEditing = false;
    }

    [RelayCommand]
    private void ToggleLayoutEdit() => IsLayoutEditing = !IsLayoutEditing;

    private async Task LoadGalleryImagesAsync(Project project)
    {
        GalleryImages.Clear();

        var photoIds = project.TimelineEntries
            .SelectMany(e => e.PhotoFileIds)
            .Concat(project.Attachments
                .Where(a => FileHelper.IsImageFile(a.FileName))
                .Select(a => a.FileId))
            .Distinct();

        foreach (var fileId in photoIds)
        {
            try
            {
                var bytes = await _fileStorage.DownloadFileAsync(fileId);
                GalleryImages.Add(bytes);
            }
            catch { /* Skip broken images */ }
        }
    }

    private void PopulateEditFields(Project project)
    {
        EditName = project.Name;
        EditDescription = project.Description;
        EditPrice = project.Price?.ToString("F2") ?? string.Empty;
        EditStartDate = project.StartDate?.ToLocalTime();
        EditEndDate = project.EndDate?.ToLocalTime();
        EditStatus = project.Status;
    }

    [RelayCommand]
    private void ToggleEdit()
    {
        if (IsEditing && Project is not null)
        {
            PopulateEditFields(Project);
        }
        IsEditing = !IsEditing;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Project is null) return;

        var nameValidation = InputValidator.ValidateProjectName(EditName);
        if (!nameValidation.IsValid)
        {
            ErrorMessage = nameValidation.Error;
            return;
        }

        var descValidation = InputValidator.ValidateDescription(EditDescription);
        if (!descValidation.IsValid)
        {
            ErrorMessage = descValidation.Error;
            return;
        }

        var priceValidation = InputValidator.ValidatePrice(EditPrice);
        if (!priceValidation.IsValid)
        {
            ErrorMessage = priceValidation.Error;
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Project.Name = InputValidator.SanitizeString(EditName);
            Project.Description = InputValidator.SanitizeString(EditDescription);
            Project.Price = string.IsNullOrWhiteSpace(EditPrice) ? null : decimal.Parse(EditPrice);
            Project.StartDate = EditStartDate?.ToUniversalTime();
            Project.EndDate = EditEndDate?.ToUniversalTime();
            Project.Status = EditStatus;

            if (EditStatus == ProjectStatus.InProgress && Project.StartDate is null)
                Project.StartDate = DateTime.UtcNow;

            if (EditStatus == ProjectStatus.Completed && Project.EndDate is null)
                Project.EndDate = DateTime.UtcNow;

            await _projectRepo.UpdateAsync(Project);
            IsEditing = false;
            await LoadProjectAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd zapisu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ChangeStatusAsync(ProjectStatus newStatus)
    {
        if (Project is null) return;

        Project.Status = newStatus;

        if (newStatus == ProjectStatus.InProgress && Project.StartDate is null)
        {
            Project.StartDate = DateTime.UtcNow;
            Project.TimelineEntries.Add(new TimelineEntry
            {
                Title = "Rozpoczęcie projektu",
                Description = "Projekt został rozpoczęty.",
                Type = TimelineEntryType.Start,
                Date = DateTime.UtcNow
            });
        }

        if (newStatus == ProjectStatus.Completed && Project.EndDate is null)
        {
            Project.EndDate = DateTime.UtcNow;
            Project.TimelineEntries.Add(new TimelineEntry
            {
                Title = "Zakończenie projektu",
                Description = "Projekt został ukończony.",
                Type = TimelineEntryType.Completion,
                Date = DateTime.UtcNow
            });
        }

        await _projectRepo.UpdateAsync(Project);
        await LoadProjectAsync();
    }

    [RelayCommand]
    private async Task AddPhotoAsync()
    {
        if (Project is null) return;

        var dialog = new OpenFileDialog
        {
            Filter = "Obrazy|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp|Wszystkie pliki|*.*",
            Multiselect = true,
            Title = "Dodaj zdjęcia"
        };

        if (dialog.ShowDialog() != true) return;

        IsBusy = true;

        try
        {
            var photoFileIds = new List<string>();

            foreach (var filePath in dialog.FileNames)
            {
                var validation = FileHelper.ValidateFile(filePath);
                if (!validation.IsValid)
                {
                    ErrorMessage = $"{Path.GetFileName(filePath)}: {validation.Error}";
                    continue;
                }

                var fileName = FileHelper.SanitizeFileName(Path.GetFileName(filePath));
                var contentType = FileHelper.GetContentType(fileName);

                await using var stream = File.OpenRead(filePath);
                var fileId = await _fileStorage.UploadFileAsync(stream, fileName, contentType, Project.Id, "photo");
                photoFileIds.Add(fileId);
            }

            if (photoFileIds.Count > 0)
            {
                var entry = new TimelineEntry
                {
                    Title = $"Dodano {photoFileIds.Count} zdjęć",
                    Description = string.Empty,
                    Type = TimelineEntryType.Photo,
                    Date = DateTime.UtcNow,
                    PhotoFileIds = photoFileIds
                };

                Project.TimelineEntries.Add(entry);
                await _projectRepo.UpdateAsync(Project);
                await LoadProjectAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd dodawania zdjęć: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddAttachmentAsync()
    {
        if (Project is null) return;

        var dialog = new OpenFileDialog
        {
            Filter = "Wszystkie pliki|*.*",
            Multiselect = true,
            Title = "Dodaj pliki"
        };

        if (dialog.ShowDialog() != true) return;

        IsBusy = true;

        try
        {
            foreach (var filePath in dialog.FileNames)
            {
                var validation = FileHelper.ValidateFile(filePath);
                if (!validation.IsValid)
                {
                    ErrorMessage = $"{Path.GetFileName(filePath)}: {validation.Error}";
                    continue;
                }

                var fileName = FileHelper.SanitizeFileName(Path.GetFileName(filePath));
                var contentType = FileHelper.GetContentType(fileName);
                var fileInfo = new FileInfo(filePath);

                await using var stream = File.OpenRead(filePath);
                var fileId = await _fileStorage.UploadFileAsync(stream, fileName, contentType, Project.Id, "attachment");

                Project.Attachments.Add(new ProjectAttachment
                {
                    FileName = fileName,
                    FileId = fileId,
                    ContentType = contentType,
                    FileSizeBytes = fileInfo.Length
                });
            }

            await _projectRepo.UpdateAsync(Project);
            await LoadProjectAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd dodawania plików: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DownloadAttachmentAsync(ProjectAttachment attachment)
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                FileName = attachment.FileName,
                Title = "Zapisz plik"
            };

            if (dialog.ShowDialog() != true) return;

            var bytes = await _fileStorage.DownloadFileAsync(attachment.FileId);
            await File.WriteAllBytesAsync(dialog.FileName, bytes);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd pobierania: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteAttachmentAsync(ProjectAttachment attachment)
    {
        if (Project is null) return;

        try
        {
            await _fileStorage.DeleteFileAsync(attachment.FileId);
            Project.Attachments.Remove(attachment);
            await _projectRepo.UpdateAsync(Project);
            Attachments.Remove(attachment);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd usuwania: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task AddSectionAsync(string title)
    {
        if (Project is null || string.IsNullOrWhiteSpace(title)) return;

        var section = new ProjectSection
        {
            Title = InputValidator.SanitizeString(title),
            Order = Project.CustomSections.Count
        };

        Project.CustomSections.Add(section);
        await _projectRepo.UpdateAsync(Project);
        CustomSections.Add(section);
        BuildSectionCards();
    }

    [RelayCommand]
    private async Task AddTimelineEntryAsync((string Title, string Description, TimelineEntryType Type) entry)
    {
        if (Project is null) return;

        var timelineEntry = new TimelineEntry
        {
            Title = InputValidator.SanitizeString(entry.Title),
            Description = InputValidator.SanitizeString(entry.Description),
            Type = entry.Type,
            Date = DateTime.UtcNow
        };

        Project.TimelineEntries.Add(timelineEntry);
        await _projectRepo.UpdateAsync(Project);
        await LoadProjectAsync();
    }

    [RelayCommand]
    private async Task DeleteProjectAsync()
    {
        if (Project is null) return;

        IsBusy = true;

        try
        {
            await _fileStorage.DeleteAllForProjectAsync(Project.Id);
            await _projectRepo.DeleteAsync(Project.Id);
            _navigation.NavigateTo<ProjectListViewModel>();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd usuwania: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SwitchTab(string tab)
    {
        if (int.TryParse(tab, out var t))
            ActiveTab = t;
    }

    [RelayCommand]
    private void NavigateToNewProject() => _navigation.NavigateTo<NewProjectViewModel>();

    [RelayCommand]
    private void GoBack() => _navigation.GoBack();
}
