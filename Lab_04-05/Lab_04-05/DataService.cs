using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Lab_04_05
{
    public static class DataService
    {
        // Файл хранения данных приложения.
        private const string FilePath = "services.xml";

        public static List<DrivingService> LoadServices()
        {
            if (File.Exists(FilePath))
            {
                var serializer = new XmlSerializer(typeof(List<DrivingService>));
                using var reader = new StreamReader(FilePath);
                return (List<DrivingService>)serializer.Deserialize(reader) ?? new();
            }

            var services = GetDefaultServices();
            SaveServices(services);
            return services;
        }

        public static void SaveServices(List<DrivingService> services)
        {
            var serializer = new XmlSerializer(typeof(List<DrivingService>));
            using var writer = new StreamWriter(FilePath);
            serializer.Serialize(writer, services);
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
                    ImagePaths = new() { "Images/car_b.jpg" }
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
                    ImagePaths = new() { "Images/moto_a.jpg" }
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
                    ImagePaths = new() { "Images/advanced.jpg" }
                }
            };
        }
    }
}