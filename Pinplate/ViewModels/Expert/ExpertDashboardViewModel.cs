using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pinplate.Views.Expert;
using Pinplate.Views.Settings;

namespace Pinplate.ViewModels.Expert;

public partial class ExpertDashboardViewModel : ObservableObject
{
    [ObservableProperty] private object? currentView;
    [ObservableProperty] private string currentSectionTitle = "На оценку";
    [ObservableProperty] private string activeMenuItem = "Review";

    public ExpertDashboardViewModel() => Navigate("Review");

    [RelayCommand]
    private void Navigate(string destination)
    {
        ActiveMenuItem = destination;

        (CurrentView, CurrentSectionTitle) = destination switch
        {
            "Review" => (new ReviewView(), "На оценку"),
            "Top" => (new TopView(), "Топ-лента"),
            "Rejected" => (new RejectedView(), "Отклонено"),
            "Settings" => (new ExpertSettingsView(), "Настройки"),
            _ => (CurrentView, CurrentSectionTitle)
        };
    }

    // ДОБАВЛЯЕМ МЕТОД ПОИСКА
    public void GlobalSearch(string query)
    {
        if (CurrentView is TopView topView && topView.DataContext is ExpertTopViewModel topVm)
            topVm.SearchQuery = query;
        else if (CurrentView is ReviewView reviewView && reviewView.DataContext is ExpertReviewViewModel reviewVm)
            reviewVm.SearchQuery = query;
    }
}