using System.Windows;
using System.Linq;

namespace Lab_04_05
{
    public partial class ServiceEditWindow : Window
    {
        // Редактируемая услуга.
        public DrivingService Service { get; set; }
        // Текстовые поля для списков (изображения и связанные услуги через ';').
        public string ImagePathsText { get; set; } = string.Empty;
        public string RelatedServiceIdsText { get; set; } = string.Empty;

        public ServiceEditWindow(DrivingService service, bool isNew)
        {
            InitializeComponent();
            Icon = UiAssets.CreateWindowIcon();
            Cursor = UiAssets.CreateCustomCursor();
            Service = service;
            ImagePathsText = string.Join("; ", Service.ImagePaths ?? []);
            RelatedServiceIdsText = string.Join("; ", Service.RelatedServiceIds ?? []);
            DataContext = Service;
            Title = isNew
                ? (Application.Current.TryFindResource("AddWindowTitle") as string ?? "Добавление услуги")
                : (Application.Current.TryFindResource("EditWindowTitle") as string ?? "Редактирование услуги");
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Базовая валидация полей перед сохранением.
            if (string.IsNullOrWhiteSpace(Service.FullName))
            {
                MessageBox.Show(Tr("FullNameRequired", "Введите полное название!"));
                return;
            }
            if (Service.Rating < 0 || Service.Rating > 5)
            {
                MessageBox.Show(Tr("InvalidRating", "Рейтинг должен быть в диапазоне 0..5."));
                return;
            }
            if (Service.Discount < 0 || Service.Discount > 1)
            {
                MessageBox.Show(Tr("InvalidDiscount", "Скидка должна быть в диапазоне 0..1."));
                return;
            }
            if (Service.Price <= 0)
            {
                MessageBox.Show(Tr("InvalidPrice", "Цена должна быть больше 0."));
                return;
            }
            if (Service.Quantity < 0)
            {
                MessageBox.Show(Tr("InvalidQuantity", "Количество мест не может быть отрицательным."));
                return;
            }
            if (Service.DurationHours <= 0)
            {
                MessageBox.Show(Tr("InvalidDuration", "Длительность должна быть больше 0."));
                return;
            }
            if (Service.GroupSize <= 0)
            {
                MessageBox.Show(Tr("InvalidGroupSize", "Размер группы должен быть больше 0."));
                return;
            }

            Service.ImagePaths = ImagePathsText
                .Split(';')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();
            // Связанные услуги сохраняем списком ID.
            Service.RelatedServiceIds = RelatedServiceIdsText
                .Split(';')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static string Tr(string key, string fallback)
            => Application.Current.TryFindResource(key) as string ?? fallback;
    }
}