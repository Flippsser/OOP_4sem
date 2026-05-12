using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Lab_04_05
{
    public class MainViewModel : BaseViewModel
    {
        public bool IsAdmin => SessionContext.CurrentUser?.Role == "Admin";

        public UndoRedoManager UndoRedo { get; } = new();

        private ObservableCollection<DrivingService> _services = new();
        public ObservableCollection<DrivingService> Services
        {
            get => _services;
            set { _services = value; OnPropertyChanged(); }
        }

        public ObservableCollection<DrivingService> FilteredServices { get; } = new();

        private DrivingService _selectedService;
        public DrivingService SelectedService
        {
            get => _selectedService;
            set
            {
                if (SetProperty(ref _selectedService, value))
                {
                    LogServiceChange(value);
                }
            }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) ApplyFilters(); }
        }

        private string _selectedCategory = "Все";
        public string SelectedCategory
        {
            get => _selectedCategory;
            set { if (SetProperty(ref _selectedCategory, value)) ApplyFilters(); }
        }

        private decimal? _minPrice;
        public decimal? MinPrice
        {
            get => _minPrice;
            set { if (SetProperty(ref _minPrice, value)) ApplyFilters(); }
        }

        private decimal? _maxPrice;
        public decimal? MaxPrice
        {
            get => _maxPrice;
            set { if (SetProperty(ref _maxPrice, value)) ApplyFilters(); }
        }

        public ObservableCollection<string> Categories { get; } = new();

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SwitchLanguageCommand { get; }
        public ICommand PurchaseCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand OpenPersonalAccountCommand { get; }
        public ICommand OpenUserManagementCommand { get; }

        public string CurrentRole => IsAdmin
            ? Tr("AdminRole", "Администратор")
            : Tr("ClientRole", "Клиент");

        public string CurrentUserDisplay => $"{SessionContext.CurrentUser?.FullName ?? ""} ({CurrentRole})";

        public MainViewModel()
        {
            LoadData();

            Categories.Add(Tr("AllCategories", "Все"));
            foreach (var c in Services.Select(s => s.Category).Distinct())
                Categories.Add(c);
            SelectedCategory = Tr("AllCategories", "Все");

            AddCommand = new RelayCommand(_ => AddService(), _ => IsAdmin);
            EditCommand = new RelayCommand(_ => EditService(), _ => IsAdmin && SelectedService != null);
            DeleteCommand = new RelayCommand(_ => DeleteService(), _ => IsAdmin && SelectedService != null);
            RefreshCommand = new RelayCommand(_ => LoadData());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            SwitchLanguageCommand = new RelayCommand(param =>
            {
                var lang = param?.ToString() ?? "ru";
                LocalizationManager.SwitchLanguage(lang);
                if (SessionContext.CurrentUser != null)
                {
                    SessionContext.CurrentUser.Language = lang;
                    DataService.SaveUser(SessionContext.CurrentUser);
                }
                RefreshCategories();
                if (!Categories.Contains(SelectedCategory))
                    SelectedCategory = Tr("AllCategories", "Все");
                OnPropertyChanged(nameof(CurrentRole));
                OnPropertyChanged(nameof(CurrentUserDisplay));
                ApplyFilters();
            });
            PurchaseCommand = new RelayCommand(_ => Purchase(), _ =>
                !IsAdmin && SelectedService != null && SelectedService.Quantity > 0 && !SelectedService.IsNotAvailable);

            UndoCommand = new RelayCommand(_ =>
            {
                UndoRedo.Undo();
                CommandManager.InvalidateRequerySuggested();
            }, _ => UndoRedo.CanUndo);

            RedoCommand = new RelayCommand(_ =>
            {
                UndoRedo.Redo();
                CommandManager.InvalidateRequerySuggested();
            }, _ => UndoRedo.CanRedo);

            OpenPersonalAccountCommand = new RelayCommand(_ =>
            {
                new PersonalAccountWindow().ShowDialog();
                OnPropertyChanged(nameof(CurrentUserDisplay));
            });

            OpenUserManagementCommand = new RelayCommand(_ =>
            {
                new UserManagementWindow().ShowDialog();
            }, _ => IsAdmin);

            ApplyFilters();
        }

        public void RefreshAfterChange()
        {
            DataService.SaveServices(Services.ToList());
            RefreshCategories();
            ApplyFilters();
        }

        private void LoadData()
        {
            var list = DataService.LoadServices();
            Services = new ObservableCollection<DrivingService>(list);
            RefreshCategories();
            if (!Categories.Contains(SelectedCategory))
                SelectedCategory = Tr("AllCategories", "Все");
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (Services == null) return;
            var q = Services.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchText))
                q = q.Where(s => s.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                              || s.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            if (SelectedCategory != Tr("AllCategories", "Все")) q = q.Where(s => s.Category == SelectedCategory);
            if (MinPrice.HasValue) q = q.Where(s => s.Price >= MinPrice.Value);
            if (MaxPrice.HasValue) q = q.Where(s => s.Price <= MaxPrice.Value);

            FilteredServices.Clear();
            foreach (var service in q)
                FilteredServices.Add(service);
        }

        private void AddService()
        {
            var service = new DrivingService
            {
                FullName = Tr("NewServiceFullName", "Новая услуга")
            };
            var editor = new ServiceEditWindow(service, true);
            if (editor.ShowDialog() == true)
            {
                var cmd = new AddServiceCommand(Services, service, this,
                    $"Добавление: {service.FullName}");
                UndoRedo.ExecuteCommand(cmd);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void EditService()
        {
            if (SelectedService == null) return;
            var snapshot = new DrivingService();
            CopyService(SelectedService, snapshot);
            var editor = new ServiceEditWindow(SelectedService, false);
            if (editor.ShowDialog() == true)
            {
                var edited = new DrivingService();
                CopyService(SelectedService, edited);
                var cmd = new EditServiceCommand(SelectedService, snapshot, edited, this,
                    $"Редактирование: {SelectedService.FullName}");
                UndoRedo.ExecuteCommand(cmd);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void DeleteService()
        {
            if (SelectedService == null) return;
            if (MessageBox.Show(Tr("DeleteConfirm", "Удалить выбранную услугу?"), Tr("Confirm", "Подтверждение"),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var svc = SelectedService;
                int idx = Services.IndexOf(svc);
                SelectedService = null;
                var cmd = new DeleteServiceCommand(Services, svc, idx, this,
                    $"Удаление: {svc.FullName}");
                UndoRedo.ExecuteCommand(cmd);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void Purchase()
        {
            if (SelectedService.Quantity > 0 && !SelectedService.IsNotAvailable)
            {
                var cmd = new PurchaseServiceCommand(SelectedService, this,
                    $"Покупка: {SelectedService.FullName}");
                UndoRedo.ExecuteCommand(cmd);
                CommandManager.InvalidateRequerySuggested();
                MessageBox.Show(Tr("PurchaseSuccess", "Вы успешно записались на услугу!"));
            }
        }

        private void RefreshCategories()
        {
            Categories.Clear();
            Categories.Add(Tr("AllCategories", "Все"));
            foreach (var c in Services.Select(s => s.Category).Distinct())
                Categories.Add(c);
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedCategory = Tr("AllCategories", "Все");
            MinPrice = null;
            MaxPrice = null;
            ApplyFilters();
        }

        private static void CopyService(DrivingService from, DrivingService to)
        {
            to.Id = from.Id;
            to.FullName = from.FullName;
            to.Description = from.Description;
            to.Category = from.Category;
            to.Rating = from.Rating;
            to.Price = from.Price;
            to.Quantity = from.Quantity;
            to.TransmissionType = from.TransmissionType;
            to.DurationHours = from.DurationHours;
            to.Country = from.Country;
            to.Discount = from.Discount;
            to.IsNotAvailable = from.IsNotAvailable;
            to.PurchasedCount = from.PurchasedCount;
            to.Producer = from.Producer;
            to.Color = from.Color;
            to.GroupSize = from.GroupSize;
            to.ImagePaths = new System.Collections.Generic.List<string>(from.ImagePaths ?? new());
            to.RelatedServiceIds = new System.Collections.Generic.List<string>(from.RelatedServiceIds ?? new());
        }

        private static void LogServiceChange(DrivingService service)
        {
            if (service == null) return;
            try
            {
                var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Выбрана услуга: {service.FullName}, Цена: {service.Price}, Количество: {service.Quantity}";
                if (service.Quantity == 0)
                    log += " [НЕТ В НАЛИЧИИ]";
                File.AppendAllText("log.txt", log + Environment.NewLine);
            }
            catch { }
        }

        private static string Tr(string key, string fallback)
            => Application.Current.TryFindResource(key) as string ?? fallback;
    }
}
