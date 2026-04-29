using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Lab_04_05
{
    // Главная ViewModel: хранит состояние экрана и команды управления услугами.
    public class MainViewModel : BaseViewModel
    {
        public bool IsAdmin { get; }

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
            set => SetProperty(ref _selectedService, value);
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

        public string CurrentRole => IsAdmin
            ? Tr("AdminRole", "Администратор")
            : Tr("ClientRole", "Клиент");

        public MainViewModel(bool isAdmin)
        {
            IsAdmin = isAdmin;
            // При старте загружаем данные из файла и подготавливаем фильтры.
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
                LocalizationManager.SwitchLanguage(param?.ToString() ?? "ru");
                RefreshCategories();
                if (!Categories.Contains(SelectedCategory))
                    SelectedCategory = Tr("AllCategories", "Все");
                OnPropertyChanged(nameof(CurrentRole));
                ApplyFilters();
            });
            PurchaseCommand = new RelayCommand(_ => Purchase(), _ =>
                !IsAdmin && SelectedService != null && SelectedService.Quantity > 0 && !SelectedService.IsNotAvailable);

            ApplyFilters();
        }

        private void LoadData()
        {
            // Полное обновление данных из services.xml.
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
            // Фильтрация выполняется поверх исходного списка Services.
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
            // Создаём пустую услугу и открываем окно редактирования.
            var service = new DrivingService
            {
                FullName = Tr("NewServiceFullName", "Новая услуга")
            };
            var editor = new ServiceEditWindow(service, true);
            if (editor.ShowDialog() == true)
            {
                Services.Add(service);
                DataService.SaveServices(Services.ToList());
                RefreshCategories();
                ApplyFilters();
            }
        }

        private void EditService()
        {
            if (SelectedService == null) return;
            var editor = new ServiceEditWindow(SelectedService, false);
            if (editor.ShowDialog() == true)
            {
                DataService.SaveServices(Services.ToList());
                ApplyFilters();
            }
        }

        private void DeleteService()
        {
            if (SelectedService == null) return;
            if (MessageBox.Show(Tr("DeleteConfirm", "Удалить выбранную услугу?"), Tr("Confirm", "Подтверждение"),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Services.Remove(SelectedService);
                DataService.SaveServices(Services.ToList());
                RefreshCategories();
                ApplyFilters();
            }
        }

        private void Purchase()
        {
            // Покупка/запись на услугу доступна только клиенту и только при наличии мест.
            if (SelectedService.Quantity > 0 && !SelectedService.IsNotAvailable)
            {
                SelectedService.Quantity--;
                SelectedService.PurchasedCount++;
                MessageBox.Show(Tr("PurchaseSuccess", "Вы успешно записались на услугу!"));
                DataService.SaveServices(Services.ToList());
                ApplyFilters();
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
            // Полный сброс фильтров до состояния "показывать всё".
            SearchText = string.Empty;
            SelectedCategory = Tr("AllCategories", "Все");
            MinPrice = null;
            MaxPrice = null;
            ApplyFilters();
        }

        private static string Tr(string key, string fallback)
            => Application.Current.TryFindResource(key) as string ?? fallback;
    }
}