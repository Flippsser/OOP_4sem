using System.Windows;

namespace Lab_04_05
{
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow()
        {
            InitializeComponent();
            Icon = UiAssets.CreateWindowIcon();
            Cursor = UiAssets.CreateCustomCursor();
            DataContext = new UserManagementViewModel();
        }
    }
}
