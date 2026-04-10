using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectManager.Services;

namespace ProjectManager.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IConnectionStringProtector _protector;
    private readonly IMongoDbService _mongoDb;

    [ObservableProperty]
    private string _connectionString = string.Empty;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _isTesting;

    public SettingsViewModel(IConnectionStringProtector protector, IMongoDbService mongoDb)
    {
        _protector = protector;
        _mongoDb = mongoDb;
        IsConnected = mongoDb.IsConnected;

        var saved = _protector.Load();
        if (saved is not null)
            ConnectionString = saved;
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            StatusMessage = "Wprowadź connection string.";
            return;
        }

        IsTesting = true;
        StatusMessage = "Testowanie połączenia...";

        try
        {
            var success = await _mongoDb.TestConnectionAsync(ConnectionString);

            if (success)
                StatusMessage = "Połączenie udane!";
            else
                StatusMessage = "Nie udało się połączyć z bazą danych.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Błąd: {ex.Message}";
        }
        finally
        {
            IsTesting = false;
        }
    }

    [RelayCommand]
    private async Task SaveConnectionAsync()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            StatusMessage = "Wprowadź connection string.";
            return;
        }

        IsTesting = true;
        StatusMessage = "Zapisywanie...";

        try
        {
            var success = await _mongoDb.TestConnectionAsync(ConnectionString);

            if (!success)
            {
                StatusMessage = "Nie udało się połączyć. Sprawdź connection string.";
                return;
            }

            _protector.Save(ConnectionString);
            _mongoDb.Initialize(ConnectionString);
            IsConnected = true;
            StatusMessage = "Połączenie zapisane i aktywne.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Błąd: {ex.Message}";
        }
        finally
        {
            IsTesting = false;
        }
    }

    [RelayCommand]
    private void ClearConnection()
    {
        _protector.Delete();
        ConnectionString = string.Empty;
        IsConnected = false;
        StatusMessage = "Connection string usunięty.";
    }
}
