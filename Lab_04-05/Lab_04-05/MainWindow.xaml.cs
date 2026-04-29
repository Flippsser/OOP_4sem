using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab_04_05
{
    public partial class MainWindow : Window
    {
        public MainWindow(bool isAdmin)
        {
            InitializeComponent();
            // Подключаем ViewModel главного экрана с учётом роли пользователя.
            DataContext = new MainViewModel(isAdmin);
            Icon = UiAssets.CreateWindowIcon();
            Cursor = UiAssets.CreateCustomCursor();
        }

        // Разрешаем ввод только цифр в фильтрах цены.
        private void PriceFilterTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigitsOnly(e.Text);
        }

        // Блокируем вставку нецифровых значений (Ctrl+V).
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
    }
}