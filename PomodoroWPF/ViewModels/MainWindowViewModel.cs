using PomodoroWPF.ViewModels.Base;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace PomodoroWPF.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            InitCommands();
        }
        public void InitCommands()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            TimerCurrentTime = 5 + 1;
            StopTimerCommand = new DelegateCommand(c => {
                TimerCurrentTime = 5 + 1;
                timer.Stop();
                TimeString = ("00" + (TimerTime / 60).ToString())[^2..] + ":" + ("00" + (TimerTime % 60).ToString())[^2..];
                if (IsPlayed)
                    IsPlayed = false;
            });

            StartPauseTimerCommand = new DelegateCommand(c => {
                if (IsPlayed)
                {
                    timer.Stop();
                    IsPlayed = false;
                }
                else
                {
                    timer.Start();
                    IsPlayed = true;
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


        private string timeString = ("00" + (TimerTime / 60).ToString())[^2..] + ":" + ("00" + (TimerTime % 60).ToString())[^2..];
        private DispatcherTimer timer;

        public string TimeString
        {
            get => timeString;
            set
            {
                SetProperty(ref timeString, value);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (TimerCurrentTime != 0)
            {
                TimerCurrentTime -= 1;
                int minutes = TimerCurrentTime / 60;
                int seconds = TimerCurrentTime % 60;
                TimeString = ("00" + minutes.ToString())[^2..] + ":" + ("00" + seconds.ToString())[^2..];
            }
            else
            {
                timer.Stop();
                TimerStatus = "Rest time";
                TimerCurrentTime = 300;
                timer.Start();
            }
        }

        public bool IsPlayed = false;
        public int TimerCurrentTime { get; set; }
        public static int TimerTime = 5;
    }
}
