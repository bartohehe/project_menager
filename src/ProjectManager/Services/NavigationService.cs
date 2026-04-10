using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectManager.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<ObservableObject> _history = new();

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ObservableObject CurrentView { get; private set; } = null!;
    public event Action<ObservableObject>? CurrentViewChanged;

    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        if (CurrentView is not null)
            _history.Push(CurrentView);

        CurrentView = _serviceProvider.GetRequiredService<TViewModel>();
        CurrentViewChanged?.Invoke(CurrentView);
    }

    public void NavigateTo<TViewModel>(object parameter) where TViewModel : ObservableObject
    {
        if (CurrentView is not null)
            _history.Push(CurrentView);

        var vm = _serviceProvider.GetRequiredService<TViewModel>();
        CurrentView = vm;

        if (vm is IParameterReceiver receiver)
            receiver.ReceiveParameter(parameter);

        CurrentViewChanged?.Invoke(CurrentView);
    }

    public void GoBack()
    {
        if (_history.TryPop(out var previous))
        {
            CurrentView = previous;
            CurrentViewChanged?.Invoke(CurrentView);
        }
    }
}
