using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lab_04_05
{
    public class TransmissionDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value?.ToString() ?? string.Empty;
            return text switch
            {
                "МКПП" => Application.Current.TryFindResource("TransmissionManual") as string ?? "Manual",
                "АКПП" => Application.Current.TryFindResource("TransmissionAuto") as string ?? "Automatic",
                _ => text
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
