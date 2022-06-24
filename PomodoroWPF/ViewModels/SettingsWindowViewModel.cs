using PomodoroWPF.ViewModels.Base;

namespace PomodoroWPF.ViewModels
{
    internal class SettingsWindowViewModel : ObservableObject
    {
        private string workMinutes = "25";

        public string WorkMinutes
        {
            get => workMinutes;
            set
            {
                SetProperty(ref workMinutes, value);
            }
        }

        private string workSeconds = "00";

        public string WorkSeconds
        {
            get => workSeconds;
            set
            {
                SetProperty(ref workSeconds, value);
            }
        }

        private string restMinutes = "05";

        public string RestMinutes
        {
            get => restMinutes;
            set
            {
                SetProperty(ref restMinutes, value);
            }
        }

        private string restSeconds = "00";

        public string RestSeconds
        {
            get => restSeconds;
            set
            {
                SetProperty(ref restSeconds, value);
            }
        }
    }
}
