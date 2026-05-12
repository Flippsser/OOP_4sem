using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Lab_04_05.Controls
{
    public partial class ThemeToggle : UserControl
    {
        public static readonly DependencyProperty IsDarkThemeProperty =
            DependencyProperty.Register(nameof(IsDarkTheme), typeof(bool), typeof(ThemeToggle),
                new PropertyMetadata(false, OnIsDarkThemeChanged));

        public bool IsDarkTheme
        {
            get => (bool)GetValue(IsDarkThemeProperty);
            set => SetValue(IsDarkThemeProperty, value);
        }

        public ThemeToggle()
        {
            InitializeComponent();
        }

        private static void OnIsDarkThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemeToggle toggle)
                toggle.AnimateToggle((bool)e.NewValue);
        }

        private void AnimateToggle(bool isDark)
        {
            if (isDark)
            {
                var marginAnim = new ThicknessAnimation
                {
                    To = new Thickness(36, 0, 0, 0),
                    Duration = new Duration(new(0, 0, 0, 0, 200)),
                    EasingFunction = new QuadraticEase()
                };
                Thumb.BeginAnimation(MarginProperty, marginAnim);

                Track.Background = new SolidColorBrush(Color.FromRgb(0x42, 0x42, 0x42));
                Thumb.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xCC, 0x66));
                IconLeft.Foreground = new SolidColorBrush(Color.FromRgb(0x90, 0x90, 0x90));
                IconRight.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xCC, 0x66));
            }
            else
            {
                var marginAnim = new ThicknessAnimation
                {
                    To = new Thickness(4, 0, 0, 0),
                    Duration = new Duration(new(0, 0, 0, 0, 200)),
                    EasingFunction = new QuadraticEase()
                };
                Thumb.BeginAnimation(MarginProperty, marginAnim);

                Track.Background = new SolidColorBrush(Color.FromRgb(0xBB, 0xDE, 0xFB));
                Thumb.Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
                IconLeft.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xA7, 0x26));
                IconRight.Foreground = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
            }
        }
    }
}
