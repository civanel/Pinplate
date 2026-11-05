using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Pinplate.ViewModels.Settings;

public partial class ExpertProfileSectionViewModel : ObservableObject
{
    [ObservableProperty] private string userName = "Эксперт";
    [ObservableProperty] private string email = "expert@pinplate.app";

    [RelayCommand] private void ChangePhoto() { }
    [RelayCommand] private void Save() { }
}