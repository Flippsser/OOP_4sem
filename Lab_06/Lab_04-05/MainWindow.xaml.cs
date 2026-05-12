using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lab_04_05.Controls;

namespace Lab_04_05
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            Icon = UiAssets.CreateWindowIcon();
            Cursor = UiAssets.CreateCustomCursor();
            ThemeToggleControl.MouseLeftButtonUp += ThemeToggle_MouseLeftButtonUp;
        }

        private void PriceFilterTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigitsOnly(e.Text);
        }

        private void PriceFilterTextBox_OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            var text = (string)e.DataObject.GetData(typeof(string));
            if (!IsDigitsOnly(text))
                e.CancelCommand();
        }

        private static bool IsDigitsOnly(string text)
        {
            foreach (var c in text)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }

        private void ThemeToggle_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            var toggle = ThemeToggleControl;
            if (toggle == null) return;
            var isDark = !toggle.IsDarkTheme;
            toggle.IsDarkTheme = isDark;
            var theme = isDark ? "Dark" : "Light";
            ThemeManager.SwitchTheme(theme);
            if (SessionContext.CurrentUser != null)
            {
                SessionContext.CurrentUser.Theme = theme;
                DataService.SaveUser(SessionContext.CurrentUser);
            }
        }
    }
}
