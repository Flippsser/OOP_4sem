using System.IO;
using System.Windows;

namespace Lab_04_05
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _vm;

        public LoginWindow()
        {
            InitializeComponent();
            Icon = UiAssets.CreateWindowIcon();
            Cursor = UiAssets.CreateCustomCursor();
            _vm = new LoginViewModel();
            DataContext = _vm;
            _vm.LoginCompleted += (_, _) =>
            {
                try
                {
                    new MainWindow().Show();
                    Close();
                }
                catch (Exception ex)
                {
                    try { File.WriteAllText("mainwindow_error.txt", ex.ToString()); } catch { }
                    MessageBox.Show($"Ошибка при открытии главного окна:\n{ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.Password = PasswordBox.Password;
        }
    }
}
