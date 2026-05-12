using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Lab_04_05
{
    public class NoImagesVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paths = value as IEnumerable<string>;
            if (paths == null) return Visibility.Visible;

            bool hasAnyExisting = paths.Any(ImagePathToSourceConverter.PathExists);
            return hasAnyExisting ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
