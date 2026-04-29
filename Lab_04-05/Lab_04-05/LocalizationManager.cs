using System;
using System.Windows;

namespace Lab_04_05
{
    // Переключает языковой словарь приложения во время работы.
    public static class LocalizationManager
    {
        public static string CurrentLanguage { get; private set; } = "ru";

        public static void SwitchLanguage(string lang)
        {
            if (CurrentLanguage == lang) return;
            CurrentLanguage = lang;

            var dicts = Application.Current.Resources.MergedDictionaries;
            // Удаляем предыдущий словарь строк (Strings.ru/en.xaml).
            for (int i = dicts.Count - 1; i >= 0; i--)
                if (dicts[i].Source != null && dicts[i].Source.OriginalString.Contains("Strings."))
                    dicts.RemoveAt(i);

            var uri = new Uri($"Strings.{lang}.xaml", UriKind.Relative);
            dicts.Add(new ResourceDictionary { Source = uri });
        }
    }
}