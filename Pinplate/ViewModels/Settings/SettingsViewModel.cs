using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pinplate.Views.Settings.Sections;

namespace Pinplate.ViewModels.Settings;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private object? currentSection;

    [ObservableProperty]
    private string activeSection = "Security";

    public SettingsViewModel()
    {
        Navigate("Security");
    }

    [RelayCommand]
    private void Navigate(string section)
    {
        ActiveSection = section;

        CurrentSection = section switch
        {
            "Security" => new SecuritySection(),
            "Notifications" => new NotificationsSection(),
            "Appearance" => new AppearanceSection(),
            "Profile" => new ProfileSection(),
            _ => CurrentSection
        };
    }
}