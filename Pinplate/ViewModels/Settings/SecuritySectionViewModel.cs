using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pinplate.Views.Settings.Sections;
using System;
using System.Windows;

namespace Pinplate.ViewModels.Settings;

public partial class SecuritySectionViewModel : ObservableObject
{
    [ObservableProperty] private DateTime lastPasswordChange = new DateTime(2026, 8, 15);
    [ObservableProperty] private bool isTwoFactorEnabled;

    [RelayCommand]
    private void ChangePassword()
    {
        var dialog = new ChangePasswordDialog();
        dialog.Owner = Application.Current.MainWindow;
        dialog.ShowDialog();
    }

    [RelayCommand]
    private void LogoutAll()
    {
        MessageBox.Show("Вы вышли со всех устройств", "Готово");
    }
}