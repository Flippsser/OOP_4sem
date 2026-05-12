using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace Lab_04_05
{
    public static class DataService
    {
        private const string FilePath = "appdata.xml";
        private static AppData _cache;

        public static AppData LoadAppData()
        {
            if (_cache != null) return _cache;
            if (File.Exists(FilePath))
            {
                var serializer = new XmlSerializer(typeof(AppData));
                using var reader = new StreamReader(FilePath);
                _cache = (AppData)serializer.Deserialize(reader) ?? GetDefaultAppData();
                return _cache;
            }
            _cache = GetDefaultAppData();
            SaveAppData();
            return _cache;
        }

        public static void SaveAppData()
        {
            if (_cache == null) return;
            var serializer = new XmlSerializer(typeof(AppData));
            using var writer = new StreamWriter(FilePath);
            serializer.Serialize(writer, _cache);
        }

        public static List<DrivingService> LoadServices()
        {
            return LoadAppData().Services;
        }

        public static void SaveServices(List<DrivingService> services)
        {
            LoadAppData().Services = services;
            SaveAppData();
        }

        public static List<UserData> LoadUsers()
        {
            return LoadAppData().Users;
        }

        public static void SaveUsers(List<UserData> users)
        {
            LoadAppData().Users = users;
            SaveAppData();
        }

        public static UserData Authenticate(string login, string password)
        {
            var users = LoadUsers();
            return users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public static void SaveUser(UserData user)
        {
            var users = LoadUsers();
            var idx = users.FindIndex(u => u.Id == user.Id);
            if (idx >= 0)
                users[idx] = user;
            SaveUsers(users);
        }

        private static AppData GetDefaultAppData()
        {
            var users = new List<UserData>
            {
                new()
                {
                    Id = "1",
                    Login = "admin",
                    Password = "admin123",
                    FullName = "Иван Иванов",
                    Description = "Администратор системы",
                    Role = "Admin",
                    Theme = "Light",
                    Language = "ru",
                    Profile = new ProfileData { FullName = "Иван Иванов", Email = "ivan@mail.ru", Phone = "+375291234567" }
                },
                new()
                {
                    Id = "2",
                    Login = "client",
                    Password = "client123",
                    FullName = "Пётр Петров",
                    Description = "Клиент",
                    Role = "Client",
                    Theme = "Light",
                    Language = "ru",
                    Profile = new ProfileData { FullName = "Пётр Петров", Email = "petr@mail.ru", Phone = "+375291112233" }
                }
            };
            return new AppData { Services = GetDefaultServices(), Users = users };
        }

        private static List<DrivingService> GetDefaultServices()
        {
            return new List<DrivingService>
            {
                new()
                {
                    Id = "1",
                    FullName = "Полный курс вождения категории B",
                    Description = "56 ч теории, 28 ч практики. Автомобиль с МКПП.",
                    Category = "Категория B",
                    Rating = 4.5,
                    Price = 35000,
                    Quantity = 50,
                    TransmissionType = "МКПП",
                    DurationHours = 84,
                    Country = "РБ",
                    Discount = 0.05m,
                    IsNotAvailable = false,
                    PurchasedCount = 120,
                    Producer = "Автошкола 'Драйв'",
                    Color = "Белый",
                    GroupSize = 12,
                    ImagePaths = new() { "Img/car_b.jpg" }
                },
                new()
                {
                    Id = "2",
                    FullName = "Обучение на категорию A",
                    Description = "16 часов практического вождения мотоцикла.",
                    Category = "Мотоцикл A",
                    Rating = 4.7,
                    Price = 25000,
                    Quantity = 30,
                    TransmissionType = "МКПП",
                    DurationHours = 16,
                    Country = "РБ",
                    Discount = 0,
                    IsNotAvailable = false,
                    PurchasedCount = 75,
                    Producer = "Автошкола 'Драйв''",
                    Color = "Красный",
                    GroupSize = 8,
                    ImagePaths = new() { "Img/download.jpg" }
                },
                new()
                {
                    Id = "3",
                    FullName = "Контраварийная подготовка",
                    Description = "Экстремальное вождение, курс 24 часа.",
                    Category = "Повышение квалификации",
                    Rating = 4.9,
                    Price = 50000,
                    Quantity = 15,
                    TransmissionType = "АКПП",
                    DurationHours = 24,
                    Country = "РБ",
                    Discount = 0.1m,
                    IsNotAvailable = false,
                    PurchasedCount = 40,
                    Producer = "Автошкола 'Драйв'",
                    Color = "Синий",
                    GroupSize = 6,
                    ImagePaths = new() { "Img/advanced.jpg" }
                }
            };
        }
    }
}
