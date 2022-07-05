using PomodoroWPF.ViewModels.Base;
using System;
using System.Windows;
using System.Windows.Input;

namespace PomodoroWPF.ViewModels
{
    internal class SettingsWindowViewModel : ObservableObject
    {
        public SettingsWindowViewModel()
        {
            SetTimeStrings();
            InitCommands();
        }

        private void InitCommands()
        {
            ResetTime = new DelegateCommand(c =>
            {
                Properties.Settings.Default.work_time = 1500;
                Properties.Settings.Default.rest_time = 300;
                Properties.Settings.Default.Save(); 
                SetTimeStrings();
                MessageBox.Show("Время сброшено!");
            });

            AcceptTime = new DelegateCommand(c =>
            {
                int wMinutes = int.Parse(WorkMinutes);
                int wSeconds = int.Parse(WorkSeconds);
                int rMinutes = int.Parse(RestMinutes);
                int rSeconds = int.Parse(RestSeconds);
                Properties.Settings.Default.work_time = wMinutes * 60 + wSeconds;
                Properties.Settings.Default.rest_time = rMinutes * 60 + rSeconds;
                Properties.Settings.Default.Save();
                MessageBox.Show("Новое время установлено!");
            });
        }

        private void SetTimeStrings()
        {
            workMinutes = ("0" + Convert.ToString(Properties.Settings.Default.work_time / 60))[^2..];
            workSeconds = ("0" + Convert.ToString(Properties.Settings.Default.work_time % 60))[^2..];
            restMinutes = ("0" + Convert.ToString(Properties.Settings.Default.rest_time / 60))[^2..];
            restSeconds = ("0" + Convert.ToString(Properties.Settings.Default.rest_time % 60))[^2..];
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
    }
}
