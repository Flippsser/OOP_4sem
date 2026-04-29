using System;
using System.Windows.Input;

namespace Lab_04_05
{
    public class LoginViewModel : BaseViewModel
    {
        private bool _isClientSelected = true;
        public bool IsClientSelected
        {
            get => _isClientSelected;
            set
            {
                if (SetProperty(ref _isClientSelected, value) && value)
                {
                    if (_isAdminSelected)
                    {
                        _isAdminSelected = false;
                        OnPropertyChanged(nameof(IsAdminSelected));
                    }
                }
            }
        }

        private bool _isAdminSelected;
        public bool IsAdminSelected
        {
            get => _isAdminSelected;
            set
            {
                if (SetProperty(ref _isAdminSelected, value) && value)
                {
                    if (_isClientSelected)
                    {
                        _isClientSelected = false;
                        OnPropertyChanged(nameof(IsClientSelected));
                    }
                }
            }
        }
        public event EventHandler LoginCompleted;
        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => LoginCompleted?.Invoke(this, EventArgs.Empty));
        }
    }
}