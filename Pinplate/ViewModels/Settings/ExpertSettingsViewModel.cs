using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pinplate.Views.Settings.Sections;

namespace Pinplate.ViewModels.Settings;

public partial class ExpertSettingsViewModel : ObservableObject
{
    [ObservableProperty] private object? currentSection;
    [ObservableProperty] private string activeSection = "Security";

    public ExpertSettingsViewModel() => Navigate("Security");

    [RelayCommand]
    private void Navigate(string section)
    {
        ActiveSection = section;

        CurrentSection = section switch
        {
            "Security" => new SecuritySection(),
            "Notifications" => new NotificationsSection(),
            "Appearance" => new AppearanceSection(),
            "Profile" => new ExpertProfileSection(),
            _ => CurrentSection
        };
    }
}