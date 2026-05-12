using System;
using System.Linq;
using System.Windows;

namespace Lab_04_05
{
    public static class ThemeManager
    {
        public static string CurrentTheme { get; private set; } = "Light";
        public static event Action ThemeChanged;

        public static void SwitchTheme(string theme)
        {
            if (CurrentTheme == theme) return;
            CurrentTheme = theme;

            var dicts = Application.Current.Resources.MergedDictionaries;
            var toRemove = dicts
                .Where(d => d.Source != null && d.Source.OriginalString.Contains("Theme"))
                .ToList();
            foreach (var d in toRemove)
                dicts.Remove(d);

            var uri = new Uri($"Resources/{theme}Theme.xaml", UriKind.Relative);
            dicts.Add(new ResourceDictionary { Source = uri });

            ThemeChanged?.Invoke();
        }
    }
}
