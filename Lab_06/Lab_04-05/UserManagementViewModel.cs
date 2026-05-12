using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System;

namespace Lab_04_05
{
    public class UserManagementViewModel : BaseViewModel
    {
        public ObservableCollection<UserData> Users { get; } = new();

        private UserData _selectedUser;
        public UserData SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        private bool _isEditVisible;
        public bool IsEditVisible
        {
            get => _isEditVisible;
            set => SetProperty(ref _isEditVisible, value);
        }

        private bool _isEditingExisting;

        private string _editId;
        public string EditId { get => _editId; set => SetProperty(ref _editId, value); }
        private string _editLogin;
        public string EditLogin { get => _editLogin; set => SetProperty(ref _editLogin, value); }
        private string _editPassword;
        public string EditPassword { get => _editPassword; set => SetProperty(ref _editPassword, value); }
        private string _editFullName;
        public string EditFullName { get => _editFullName; set => SetProperty(ref _editFullName, value); }
        private string _editDescription;
        public string EditDescription { get => _editDescription; set => SetProperty(ref _editDescription, value); }
        private string _editRole;
        public string EditRole { get => _editRole; set => SetProperty(ref _editRole, value); }
        private string _editTheme;
        public string EditTheme { get => _editTheme; set => SetProperty(ref _editTheme, value); }
        private string _editLanguage;
        public string EditLanguage { get => _editLanguage; set => SetProperty(ref _editLanguage, value); }

        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }

        public UserManagementViewModel()
        {
            RefreshUsers();
            AddUserCommand = new RelayCommand(_ => AddUser());
            EditUserCommand = new RelayCommand(_ => BeginEdit(), _ => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(_ => DeleteUser(), _ => SelectedUser != null);
            SaveEditCommand = new RelayCommand(_ => SaveEdit());
            CancelEditCommand = new RelayCommand(_ => CancelEdit());
        }

        private void RefreshUsers()
        {
            Users.Clear();
            foreach (var u in DataService.LoadUsers())
                Users.Add(u);
        }

        private void AddUser()
        {
            EditId = Guid.NewGuid().ToString();
            EditLogin = "";
            EditPassword = "";
            EditFullName = "";
            EditDescription = "";
            EditRole = "Client";
            EditTheme = "Light";
            EditLanguage = "ru";
            _isEditingExisting = false;
            IsEditVisible = true;
        }

        private void BeginEdit()
        {
            if (SelectedUser == null) return;
            EditId = SelectedUser.Id;
            EditLogin = SelectedUser.Login;
            EditPassword = SelectedUser.Password;
            EditFullName = SelectedUser.FullName;
            EditDescription = SelectedUser.Description;
            EditRole = SelectedUser.Role;
            EditTheme = SelectedUser.Theme;
            EditLanguage = SelectedUser.Language;
            _isEditingExisting = true;
            IsEditVisible = true;
        }

        private void SaveEdit()
        {
            if (string.IsNullOrWhiteSpace(EditLogin))
            {
                System.Windows.MessageBox.Show("Login cannot be empty!");
                return;
            }

            if (!_isEditingExisting && !string.IsNullOrWhiteSpace(EditId) && Users.Any(u => u.Id == EditId))
            {
                System.Windows.MessageBox.Show("Пользователь с таким ID уже существует!");
                return;
            }

            var existing = Users.FirstOrDefault(u => u.Id == EditId);
            if (existing != null)
            {
                existing.Login = EditLogin;
                existing.Password = EditPassword;
                existing.FullName = EditFullName;
                existing.Description = EditDescription;
                existing.Role = EditRole;
                existing.Theme = EditTheme;
                existing.Language = EditLanguage;
            }
            else
            {
                var newUser = new UserData
                {
                    Id = EditId,
                    Login = EditLogin,
                    Password = EditPassword,
                    FullName = EditFullName,
                    Description = EditDescription,
                    Role = EditRole,
                    Theme = EditTheme,
                    Language = EditLanguage,
                    Profile = new ProfileData { FullName = EditFullName }
                };
                Users.Add(newUser);
            }

            DataService.SaveUsers(Users.ToList());
            RefreshUsers();
            IsEditVisible = false;
        }

        private void CancelEdit()
        {
            IsEditVisible = false;
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;
            if (SelectedUser.Id == SessionContext.CurrentUser?.Id)
            {
                System.Windows.MessageBox.Show("Нельзя удалить самого себя!");
                return;
            }
            if (System.Windows.MessageBox.Show($"Удалить пользователя {SelectedUser.Login}?", "Подтверждение",
                    System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                Users.Remove(SelectedUser);
                DataService.SaveUsers(Users.ToList());
                RefreshUsers();
            }
        }
    }
}
