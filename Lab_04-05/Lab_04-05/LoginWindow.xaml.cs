using System.Windows;

namespace Lab_04_05
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Icon = UiAssets.CreateWindowIcon();
            Cursor = UiAssets.CreateCustomCursor();
            var vm = new LoginViewModel();
            DataContext = vm;
            vm.LoginCompleted += (_, _) =>
            {
                bool isAdmin = vm.IsAdminSelected;
                new MainWindow(isAdmin).Show();
                Close();
            };
        }
    }
}