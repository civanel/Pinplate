using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pinplate.ViewModels.Settings;

public partial class AppearanceSectionViewModel : ObservableObject
{
    public ObservableCollection<string> Languages { get; } = new() { "Русский", "English" };
    public ObservableCollection<string> FontSizes { get; } = new() { "Мелкий", "Средний", "Крупный" };

    [ObservableProperty] private string selectedLanguage = "Русский";
    [ObservableProperty] private string selectedFontSize = "Средний";

    partial void OnSelectedLanguageChanged(string value)
    {
        MessageBox.Show($"Язык изменен на: {value}", "Настройки");
    }

    partial void OnSelectedFontSizeChanged(string value)
    {
        MessageBox.Show($"Размер шрифта изменен на: {value}", "Настройки");
    }
}