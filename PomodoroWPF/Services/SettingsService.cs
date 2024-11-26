using System;
using System.IO;
using System.Text.Json;

namespace PomodoroWPF.Services
{
    public class SettingsService
    {
        private const string SettingsFilePath = "settings.json";
        private static SettingsService _instance;
        public static SettingsService Instance => _instance ??= new SettingsService();

        public event EventHandler SettingsChanged;

        private SettingsService() { }

        public AppSettings LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json);
            }

            return new AppSettings
            {
                WorkTimeSeconds = 1500,
                RestTimeSeconds = 300,
                SelectedTextColor = "#00FF38",
                SelectedBackgroundImagePath = "../Assets/Backgrounds/classic/bg_green.png"
            };
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