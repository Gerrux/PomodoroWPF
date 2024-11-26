using PomodoroWPF.DataModels;
using PomodoroWPF.Services;
using PomodoroWPF.ViewModels.Base;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PomodoroWPF.ViewModels
{
    internal class SettingsWindowViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly SettingsService _settingsService;
        private AppSettings _appSettings;

        public SettingsWindowViewModel()
        {
            _settingsService = SettingsService.Instance;
            _settingsService.LoadSettings();
            _appSettings = _settingsService.LoadSettings();
            SetTimeStrings();
            InitCommands();
        }

        private void InitCommands()
        {
            ResetTime = new DelegateCommand(c =>
            {
                _appSettings = new AppSettings
                {
                    WorkTimeSeconds = 1500, // 25 minutes
                    RestTimeSeconds = 300,  // 5 minutes
                    SelectedTextColor = "#00FF38", // Default Green
                    SelectedBackgroundImagePath = "../Assets/Backgrounds/classic/bg_green.png"
                };
                _settingsService.SaveSettings(_appSettings);
                SetTimeStrings();
                OnPropertyChanged(nameof(SelectedTextColor));
                OnPropertyChanged(nameof(SelectedBackgroundImagePath));
                MessageBox.Show("Время сброшено!");
            });

            AcceptTime = new DelegateCommand(c =>
            {
                if (HasErrors)
                {
                    MessageBox.Show("Пожалуйста, исправьте ошибки перед сохранением.");
                    return;
                }

                int wMinutes = int.Parse(WorkMinutes);
                int wSeconds = int.Parse(WorkSeconds);
                int rMinutes = int.Parse(RestMinutes);
                int rSeconds = int.Parse(RestSeconds);

                _appSettings.WorkTimeSeconds = wMinutes * 60 + wSeconds;
                _appSettings.RestTimeSeconds = rMinutes * 60 + rSeconds;
                _appSettings.SelectedTextColor = SelectedTextColor;
                _appSettings.SelectedBackgroundImagePath = SelectedBackgroundImagePath;

                _settingsService.SaveSettings(_appSettings);
                MessageBox.Show("Новое время установлено!");
            });

            OpenCustomColorPickerCommand = new DelegateCommand(c =>
            {
                var colorDialog = new System.Windows.Forms.ColorDialog();
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Get selected color and convert to HEX
                    var color = colorDialog.Color;
                    string hexColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

                    // Apply the custom color theme
                    SelectedTextColor = hexColor;
                    SelectedBackgroundImagePath = "../Assets/Backgrounds/classic/bg_white.png"; // Custom uses white background

                    MessageBox.Show($"Выбранный цвет: {hexColor}");
                }
            });

            ChangeThemeCommand = new DelegateCommand(c =>
            {
                if (c is string themeName)
                {
                    var selectedTheme = Themes.AvailableThemes.Find(t => t.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase));

                    if (selectedTheme != null)
                    {
                        SelectedTextColor = selectedTheme.TextColor;
                        SelectedBackgroundImagePath = selectedTheme.BackgroundImagePath;
                        MessageBox.Show($"Тема {themeName} успешно применена.");
                    }
                }
            });
        }

        private void SetTimeStrings()
        {
            WorkMinutes = ("0" + Convert.ToString(_appSettings.WorkTimeSeconds / 60))[^2..];
            WorkSeconds = ("0" + Convert.ToString(_appSettings.WorkTimeSeconds % 60))[^2..];
            RestMinutes = ("0" + Convert.ToString(_appSettings.RestTimeSeconds / 60))[^2..];
            RestSeconds = ("0" + Convert.ToString(_appSettings.RestTimeSeconds % 60))[^2..];
            SelectedTextColor = _appSettings.SelectedTextColor;
            SelectedBackgroundImagePath = _appSettings.SelectedBackgroundImagePath;
        }

        private string selectedTextColor;
        public string SelectedTextColor
        {
            get => selectedTextColor;
            set
            {
                if (SetProperty(ref selectedTextColor, value))
                {
                    _appSettings.SelectedTextColor = value;
                    _settingsService.SaveSettings(_appSettings);
                }
            }
        }

        private string selectedBackgroundImagePath;
        public string SelectedBackgroundImagePath
        {
            get => selectedBackgroundImagePath;
            set
            {
                if (SetProperty(ref selectedBackgroundImagePath, value))
                {
                    _appSettings.SelectedBackgroundImagePath = value;
                    _settingsService.SaveSettings(_appSettings);
                }
            }
        }

        private string workMinutes;
        public string WorkMinutes
        {
            get => workMinutes;
            set
            {
                SetProperty(ref workMinutes, value);
            }
        }

        private string workSeconds;
        public string WorkSeconds
        {
            get => workSeconds;
            set
            {
                SetProperty(ref workSeconds, value);
            }
        }

        private string restMinutes;
        public string RestMinutes
        {
            get => restMinutes;
            set
            {
                SetProperty(ref restMinutes, value);
            }
        }

        private string restSeconds;
        public string RestSeconds
        {
            get => restSeconds;
            set
            {
                SetProperty(ref restSeconds, value);
            }
        }

        public ICommand ResetTime { get; set; }
        public ICommand AcceptTime { get; set; }
        public ICommand OpenCustomColorPickerCommand { get; set; }
        public ICommand ChangeThemeCommand { get; set; }

        public bool HasErrors
        {
            get
            {
                foreach (var property in new[] { nameof(WorkMinutes), nameof(WorkSeconds), nameof(RestMinutes), nameof(RestSeconds) })
                {
                    if (this[property] != null)
                        return true;
                }
                return false;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                int value;

                switch (columnName)
                {
                    case nameof(WorkMinutes):
                        if (!int.TryParse(WorkMinutes, out value) || value < 0 || value > 60)
                            result = "Введите корректные минуты (0-60).";
                        break;
                    case nameof(WorkSeconds):
                        if (!int.TryParse(WorkSeconds, out value) || value < 0 || value > 59)
                            result = "Введите корректные секунды (0-59).";
                        break;
                    case nameof(RestMinutes):
                        if (!int.TryParse(RestMinutes, out value) || value < 0 || value > 60)
                            result = "Введите корректные минуты (0-60).";
                        break;
                    case nameof(RestSeconds):
                        if (!int.TryParse(RestSeconds, out value) || value < 0 || value > 59)
                            result = "Введите корректные секунды (0-59).";
                        break;
                }

                return result;
            }
        }

        public string Error => null;
    }
}
