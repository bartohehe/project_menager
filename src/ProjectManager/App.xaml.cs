using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManager.Services;
using ProjectManager.ViewModels;
using ProjectManager.Views;

namespace ProjectManager;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Global exception handlers
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        var mainWindow = new MainWindow
        {
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();

        // Try auto-connect, then navigate
        var protector = _serviceProvider.GetRequiredService<IConnectionStringProtector>();
        var mongoDb = _serviceProvider.GetRequiredService<IMongoDbService>();
        var navigation = _serviceProvider.GetRequiredService<INavigationService>();

        var savedConnection = protector.Load();
        if (savedConnection is not null)
        {
            try
            {
                mongoDb.Initialize(savedConnection);
                navigation.NavigateTo<DashboardViewModel>();
            }
            catch
            {
                navigation.NavigateTo<SettingsViewModel>();
            }
        }
        else
        {
            navigation.NavigateTo<SettingsViewModel>();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Services
        services.AddSingleton<IMongoDbService, MongoDbService>();
        services.AddSingleton<IConnectionStringProtector, ConnectionStringProtector>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IProjectRepository, ProjectRepository>();
        services.AddSingleton<ICategoryRepository, CategoryRepository>();
        services.AddSingleton<IFileStorageService, FileStorageService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ProjectListViewModel>();
        services.AddTransient<ProjectDetailViewModel>();
        services.AddTransient<NewProjectViewModel>();
        services.AddTransient<SettingsViewModel>();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        var details = new System.Text.StringBuilder();
        details.AppendLine($"Typ: {ex.GetType().FullName}");
        details.AppendLine($"Message: {ex.Message}");
        if (ex.InnerException is not null)
        {
            details.AppendLine();
            details.AppendLine($"Inner: {ex.InnerException.GetType().FullName}");
            details.AppendLine($"Inner message: {ex.InnerException.Message}");
        }
        details.AppendLine();
        details.AppendLine("Stack trace:");
        details.AppendLine(ex.StackTrace);

        try
        {
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ProjectManager", "error.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.WriteAllText(logPath, details.ToString());
        }
        catch { }

        MessageBox.Show(
            $"Wystąpił nieoczekiwany błąd:\n\n{details}",
            "Błąd aplikacji",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
