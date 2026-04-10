using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectManager.Services;

namespace ProjectManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private ObservableObject? _currentView;

    [ObservableProperty]
    private string _currentViewTitle = "Pulpit";

    [ObservableProperty]
    private bool _isMaximized;

    public MainViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        _navigation.CurrentViewChanged += OnViewChanged;
    }

    private void OnViewChanged(ObservableObject view)
    {
        CurrentView = view;
        CurrentViewTitle = view switch
        {
            DashboardViewModel => "Pulpit",
            ProjectListViewModel => "Projekty",
            NewProjectViewModel => "Nowy projekt",
            ProjectDetailViewModel => "Szczegóły projektu",
            SettingsViewModel => "Ustawienia",
            _ => ""
        };
    }

    [RelayCommand]
    private void NavigateToDashboard() => _navigation.NavigateTo<DashboardViewModel>();

    [RelayCommand]
    private void NavigateToProjectList() => _navigation.NavigateTo<ProjectListViewModel>();

    [RelayCommand]
    private void NavigateToNewProject() => _navigation.NavigateTo<NewProjectViewModel>();

    [RelayCommand]
    private void NavigateToSettings() => _navigation.NavigateTo<SettingsViewModel>();

    [RelayCommand]
    private void Minimize(Window window) => window.WindowState = WindowState.Minimized;

    [RelayCommand]
    private void ToggleMaximize(Window window)
    {
        if (window.WindowState == WindowState.Maximized)
        {
            window.WindowState = WindowState.Normal;
            IsMaximized = false;
        }
        else
        {
            window.WindowState = WindowState.Maximized;
            IsMaximized = true;
        }
    }

    [RelayCommand]
    private void Close(Window window) => window.Close();
}
