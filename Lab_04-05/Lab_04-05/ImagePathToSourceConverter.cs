using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Lab_04_05
{
    public class ImagePathToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string raw || string.IsNullOrWhiteSpace(raw))
                return null;

            var candidates = GetCandidates(raw);

            foreach (var candidate in candidates)
            {
                try
                {
                    if (!File.Exists(candidate)) continue;
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = new Uri(candidate, UriKind.Absolute);
                    bmp.EndInit();
                    bmp.Freeze();
                    return bmp;
                }
                catch
                {
                    
                }
            }

            return null;
        }

        public static bool PathExists(string rawPath)
            => GetCandidates(rawPath).Any(File.Exists);

        private static string[] GetCandidates(string rawPath)
        {
            var path = rawPath?.Trim().Trim('"') ?? string.Empty;
            if (string.IsNullOrWhiteSpace(path)) return Array.Empty<string>();
            return
            [
                path,
                Path.Combine(AppContext.BaseDirectory, path),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", path))
            ];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
