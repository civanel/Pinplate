using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Pinplate.ViewModels.Settings;

public partial class ProfileSectionViewModel : ObservableObject
{
    [ObservableProperty] private string userName = "Модератор";
    [ObservableProperty] private string email = "mod@pinplate.app";

    [RelayCommand]
    private void ChangePhoto()
    {
        MessageBox.Show("Диалог выбора фото (пока в разработке)", "Профиль");
    }

    [RelayCommand]
    private void Save()
    {
        MessageBox.Show("Профиль успешно сохранен!", "Успех");
    }
}