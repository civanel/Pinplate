using CommunityToolkit.Mvvm.ComponentModel;

namespace Pinplate.ViewModels.Settings;

public partial class NotificationsSectionViewModel : ObservableObject
{
    [ObservableProperty] private bool isEmailEnabled = true;
    [ObservableProperty] private bool isPushEnabled = true;
    [ObservableProperty] private bool isPushSecondEnabled;
    [ObservableProperty] private bool isRatingsEnabled = true;
}