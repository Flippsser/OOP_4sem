using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Lab_04_05
{
    public class LoginViewModel : BaseViewModel
    {
        private string _login = "";
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public event EventHandler LoginCompleted;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => DoLogin());
        }

        private void DoLogin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = Application.Current.TryFindResource("LoginErrorEmpty") as string ?? "Введите логин и пароль!";
                    return;
                }

                var user = DataService.Authenticate(Login, Password);
                if (user == null)
                {
                    ErrorMessage = Application.Current.TryFindResource("LoginErrorInvalid") as string ?? "Неверный логин или пароль!";
                    return;
                }

                ThemeManager.SwitchTheme(user.Theme);
                LocalizationManager.SwitchLanguage(user.Language);

                SessionContext.CurrentUser = user;
                LoginCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                try { File.WriteAllText("login_error.txt", ex.ToString()); } catch { }
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }
    }
}
