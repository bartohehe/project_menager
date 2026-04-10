using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectManager.Services;

public interface INavigationService
{
    ObservableObject CurrentView { get; }
    event Action<ObservableObject>? CurrentViewChanged;
    void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
    void NavigateTo<TViewModel>(object parameter) where TViewModel : ObservableObject;
    void GoBack();
}

public interface IParameterReceiver
{
    void ReceiveParameter(object parameter);
}
