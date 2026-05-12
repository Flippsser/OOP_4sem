using System;
using System.Windows;
using System.Windows.Input;

namespace Lab_04_05
{
    public class PersonalAccountViewModel : BaseViewModel
    {
        private string _profileFullName;
        private string _profileEmail;
        private string _profilePhone;
        private bool _isDarkTheme;
        private bool _isRussian = true;
        private bool _isEnglish;

        public string ProfileFullName
        {
            get => _profileFullName;
            set => SetProperty(ref _profileFullName, value);
        }
        public string ProfileEmail
        {
            get => _profileEmail;
            set => SetProperty(ref _profileEmail, value);
        }
        public string ProfilePhone
        {
            get => _profilePhone;
            set => SetProperty(ref _profilePhone, value);
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    var theme = value ? "Dark" : "Light";
                    ThemeManager.SwitchTheme(theme);
                    if (SessionContext.CurrentUser != null)
                    {
                        SessionContext.CurrentUser.Theme = theme;
                        DataService.SaveUser(SessionContext.CurrentUser);
                    }
                }
            }
        }

        public bool IsRussian
        {
            get => _isRussian;
            set
            {
                if (SetProperty(ref _isRussian, value) && value)
                {
                    SwitchLang("ru");
                    IsEnglish = false;
                }
            }
        }

        public bool IsEnglish
        {
            get => _isEnglish;
            set
            {
                if (SetProperty(ref _isEnglish, value) && value)
                {
                    SwitchLang("en");
                    IsRussian = false;
                }
            }
        }

        public string UserInfo { get; }

        public ICommand SaveProfileCommand { get; }

        public PersonalAccountViewModel()
        {
            var user = SessionContext.CurrentUser;
            if (user != null)
            {
                ProfileFullName = user.Profile?.FullName ?? user.FullName;
                ProfileEmail = user.Profile?.Email ?? "";
                ProfilePhone = user.Profile?.Phone ?? "";
                IsDarkTheme = user.Theme == "Dark";
                IsRussian = user.Language == "ru";
                IsEnglish = user.Language == "en";
                UserInfo = $"{user.Login} ({user.Role})";
            }
            else
            {
                UserInfo = "";
            }

            SaveProfileCommand = new RelayCommand(_ => SaveProfile());
        }

        private void SaveProfile()
        {
            var user = SessionContext.CurrentUser;
            if (user == null) return;

            if (user.Profile == null)
                user.Profile = new ProfileData();

            user.Profile.FullName = ProfileFullName;
            user.Profile.Email = ProfileEmail;
            user.Profile.Phone = ProfilePhone;
            user.FullName = ProfileFullName;

            DataService.SaveUser(user);
            MessageBox.Show(
                Application.Current.TryFindResource("ProfileSaved") as string ?? "Профиль сохранён!",
                Application.Current.TryFindResource("Confirm") as string ?? "Подтверждение");
        }

        private static void SwitchLang(string lang)
        {
            LocalizationManager.SwitchLanguage(lang);
            if (SessionContext.CurrentUser != null)
            {
                SessionContext.CurrentUser.Language = lang;
                DataService.SaveUser(SessionContext.CurrentUser);
            }
        }
    }
}
