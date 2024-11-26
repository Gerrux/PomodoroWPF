using System;
using System.IO;
using System.Text.Json;

namespace PomodoroWPF.Services
{
    public class SettingsService
    {
        private static readonly string SettingsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PomodoroWPF");
        private static readonly string SettingsFilePath = Path.Combine(SettingsFolderPath, "settings.json");
        private static SettingsService _instance;
        public static SettingsService Instance => _instance ??= new SettingsService();

        public event EventHandler SettingsChanged;

        private SettingsService()
        {
            // Создаём папку, если она отсутствует
            if (!Directory.Exists(SettingsFolderPath))
            {
                Directory.CreateDirectory(SettingsFolderPath);
            }
        }

        public AppSettings LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json);
            }

            // Если файл отсутствует, создаём его с настройками по умолчанию
            var defaultSettings = new AppSettings
            {
                WorkTimeSeconds = 1500,
                RestTimeSeconds = 300,
                SelectedTextColor = "#00FF38",
                SelectedBackgroundImagePath = "../Assets/Backgrounds/classic/bg_green.png"
            };
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

        public void SaveSettings(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class AppSettings
    {
        public int WorkTimeSeconds { get; set; }
        public int RestTimeSeconds { get; set; }
        public string SelectedTextColor { get; set; } // Для текста
        public string SelectedBackgroundImagePath { get; set; }
    }
}
