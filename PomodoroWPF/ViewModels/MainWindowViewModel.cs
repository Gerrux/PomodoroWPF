using PomodoroWPF.Services;
using PomodoroWPF.ViewModels.Base;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PomodoroWPF.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService;

            InitCommands();
        }
        public void InitCommands()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            TimerCurrentTime = WorkTime + 1;
            StopTimerCommand = new DelegateCommand(c =>
            {
                SetWorkTime();
                timer.Stop();
                if (!IsPaused)
                    IsPaused = true;
            });

            StartPauseTimerCommand = new DelegateCommand(c =>
            {
                if (IsPaused)
                {
                    timer.Start();
                    IsPaused = false;
                }
                else
                {
                    timer.Stop();
                    IsPaused = true;
                }
            });
        }


        public ICommand StopTimerCommand { get; set; }
        public ICommand StartPauseTimerCommand { get; set; }
        public ICommand CloseWindow { get; set; }


        private string timerStatus = "work time";

        public string TimerStatus
        {
            get => timerStatus;
            set
            {
                SetProperty(ref timerStatus, value);
            }
        }
        private static string SetTimeString(int time)
        {
            return ("00" + (time / 60).ToString())[^2..] + ":" + ("00" + (time % 60).ToString())[^2..];
        }

        private string timeString = SetTimeString(WorkTime);
        private DispatcherTimer timer;

        public string TimeString
        {
            get => timeString;
            set
            {
                SetProperty(ref timeString, value);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (TimerCurrentTime != 0)
            {
                TimerCurrentTime -= 1;
                TimeString = SetTimeString(TimerCurrentTime);
            }
            else
            {
                timer.Stop();
                if (IsRestTime)
                {
                    _notificationService.Notify("Work time!", "Пора работать сучечка! Солнце ещё высоко!", 3000, ToolTipIcon.None);
                    PlayAudioAsync();
                    SetWorkTime();
                    IsRestTime = false;
                }
                else
                {
                    _notificationService.Notify("Rest time!", "Время отдохнуть перед новой задачей", 3000, ToolTipIcon.None);
                    PlayAudioAsync();
                    SetRestTime();
                    IsRestTime = true;
                }
                timer.Start();
            }
        }

        private static void PlayAudioAsync()
        {
            MediaPlayer player = new();
            try
            {
                player.Open(new Uri("Assets/notify_sound.mp3", UriKind.Relative));
                player.Volume = 0.1;
                player.Play(); // асинхронное воспроизведение в отдельном потоке.
            }
            catch (FileNotFoundException)
            {
                System.Windows.MessageBox.Show("Файл не найден");
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("Не верный формат аудио.");
            }
        }

        private void SetWorkTime()
        {
            TimerStatus = "Work time";
            TimerCurrentTime = WorkTime;
            TimeString = SetTimeString(TimerCurrentTime);
        }

        private void SetRestTime()
        {
            TimerStatus = "Rest time";
            TimerCurrentTime = RestTime;
            TimeString = SetTimeString(TimerCurrentTime);
        }

        private bool isPaused = true;

        public bool IsPaused
        {
            get => isPaused;
            set
            {
                SetProperty(ref isPaused, value);
            }
        }
        public bool IsRestTime = false;
        public int TimerCurrentTime { get; set; }
        public static int WorkTime = 5;
        public static int RestTime = 300;
        private readonly INotificationService _notificationService;
    }
}
