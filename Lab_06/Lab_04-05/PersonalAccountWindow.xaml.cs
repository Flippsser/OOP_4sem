using System.Windows;
using Lab_04_05.Controls;

namespace Lab_04_05
{
    public partial class PersonalAccountWindow : Window
    {
        public PersonalAccountWindow()
        {
            InitializeComponent();
            Icon = UiAssets.CreateWindowIcon();
            Cursor = UiAssets.CreateCustomCursor();
            DataContext = new PersonalAccountViewModel();
           
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ThemeToggle_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (sender is ThemeToggle toggle)
            {
                toggle.IsDarkTheme = !toggle.IsDarkTheme;
            }
        }
    }
}
