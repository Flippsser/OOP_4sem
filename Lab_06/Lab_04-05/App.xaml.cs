using System.Windows;

namespace Lab_04_05
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Динамическая установка ресурса
            Resources["AppVersion"] = "1.0.0";
        }
    }
}
