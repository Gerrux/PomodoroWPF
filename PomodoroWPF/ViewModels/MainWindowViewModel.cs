using PomodoroWPF.DataModels;
using PomodoroWPF.Services;
using PomodoroWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PomodoroWPF.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {
        private readonly SettingsService _settingsService;
        private AppSettings _appSettings;
        private bool isMovingRight = false; // Direction: true - right, false - left
        private bool isFlipped;
        private readonly INotificationService _notificationService;
        private DispatcherTimer timer;
        private string currentGifSource = "/Assets/Animations/stay.gif";
        private string currentBackground = "/Assets/Backgrounds/classic/bg_green.png";
        private GifMode currentGifMode;
        private string timerStatus;
        private string timeString;
        private double gifPosition;
        private bool isPaused = true;
        private PomodoroState currentState;
        private List<PomodoroState> pomodoroSequence;
        private int sequenceIndex = -1;

        // Commands
        public ICommand StopTimerCommand { get; private set; }
        public ICommand StartPauseTimerCommand { get; private set; }
        public ICommand ChangeThemeCommand { get; private set; }
        public ICommand OpenCustomColorPickerCommand { get; private set; }

        // Constructor
        public MainWindowViewModel(INotificationService notificationService)
        {
            _settingsService = SettingsService.Instance;
            _settingsService.SettingsChanged += OnSettingsChanged;
            _appSettings = _settingsService.LoadSettings();
            _notificationService = notificationService;
            ApplyTheme();
            InitCommands();

            // Initialize the pomodoro sequence (default WRWRWRWR)
            pomodoroSequence = new List<PomodoroState>
            {
                PomodoroState.Work,
                PomodoroState.Rest,
                PomodoroState.Work,
                PomodoroState.Rest,
                PomodoroState.Work,
                PomodoroState.Rest,
                PomodoroState.Work,
                PomodoroState.Rest
            };

            // Start with the initial gif mode as Stay
            CurrentGifMode = GifMode.Stay;

            // Start with the first state
            AdvanceState();
        }

        private SolidColorBrush _textBrush;
        public SolidColorBrush TextBrush
        {
            get => _textBrush;
            set => SetProperty(ref _textBrush, value);
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            _appSettings = _settingsService.LoadSettings();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            try
            {
                TextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_appSettings.SelectedTextColor));
            }
            catch
            {
                // Fallback to default color if conversion fails
                TextBrush = new SolidColorBrush(Color.FromRgb(0, 255, 56)); // #00FF38
            }
            CurrentBackground = _appSettings.SelectedBackgroundImagePath;
        }

        public void RefreshSettings()
        {
            _appSettings = _settingsService.LoadSettings();
            ApplyTheme();
        }

        public string CurrentGifSource
        {
            get => currentGifSource;
            set => SetProperty(ref currentGifSource, value);
        }

        public GifMode CurrentGifMode
        {
            get => currentGifMode;
            set
            {
                SetProperty(ref currentGifMode, value);
                UpdateGifSource();
            }
        }

        public string CurrentBackground
        {
            get => currentBackground;
            set
            {
                SetProperty(ref currentBackground, value);
                UpdateGifSource();
            }
        }

        public string TimerStatus
        {
            get => timerStatus;
            set => SetProperty(ref timerStatus, value);
        }

        public string TimeString
        {
            get => timeString;
            set => SetProperty(ref timeString, value);
        }

        public double GifPosition
        {
            get => gifPosition;
            set => SetProperty(ref gifPosition, value);
        }

        public bool IsPaused
        {
            get => isPaused;
            set
            {
                if (SetProperty(ref isPaused, value))
                {
                    UpdateGifMode(GetProgress());
                }
            }
        }

        public bool IsFlipped
        {
            get => isFlipped;
            set => SetProperty(ref isFlipped, value);
        }

        public PomodoroState CurrentState
        {
            get => currentState;
            set => SetProperty(ref currentState, value);
        }

        public int TimerCurrentTime { get; set; }

        // Methods
        private void InitCommands()
        {
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += TimerTick;

            StopTimerCommand = new DelegateCommand(c =>
            {
                timer.Stop();
                IsPaused = true;
                isMovingRight = false;
                sequenceIndex = -1;
                AdvanceState();
                GifPosition = 0; // Reset position
            });

            StartPauseTimerCommand = new DelegateCommand(c =>
            {
                if (IsPaused)
                {
                    timer.Start();
                    IsPaused = false;
                    UpdateGifMode(GetProgress());
                }
                else
                {
                    timer.Stop();
                    IsPaused = true;
                    UpdateGifMode(GetProgress());
                }
            });
            ChangeThemeCommand = new DelegateCommand(ChangeTheme);
            OpenCustomColorPickerCommand = new DelegateCommand(OpenCustomColorPicker);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (TimerCurrentTime > 0)
            {
                TimerCurrentTime--;
                TimeString = SetTimeString(TimerCurrentTime);

                double progress = GetProgress();
                UpdateGifMode(progress);
                UpdateGifPosition(progress);
            }
            else
            {
                PlayAudioAsync();
                AdvanceState(false); // Automatic transition
            }
        }

        private void AdvanceState(bool isManual = true)
        {
            sequenceIndex = (sequenceIndex + 1) % pomodoroSequence.Count;
            CurrentState = pomodoroSequence[sequenceIndex];
            UpdateState(isManual);
        }

        private void UpdateState(bool isManual = true)
        {
            if (isManual)
            {
                timer.Stop();
                IsPaused = true;
            }

            if (CurrentState == PomodoroState.Work)
            {
                TimerStatus = "Work time";
                TimerCurrentTime = Properties.Settings.Default.work_time;

                // Toggle direction
                isMovingRight = !isMovingRight;

                // Set initial position based on direction
                GifPosition = isMovingRight ? 0 : 275;
            }
            else
            {
                TimerStatus = "Rest time";
                TimerCurrentTime = Properties.Settings.Default.rest_time;

                // Fix position for Rest state
                GifPosition = isMovingRight ? 275 : 0;
            }

            TimeString = SetTimeString(TimerCurrentTime);
            UpdateGifSource();
            UpdateGifMode(0);
        }

        private void UpdateGifSource()
        {
            CurrentGifSource = currentGifMode switch
            {
                GifMode.Stay => "/Assets/Animations/stay.gif",
                GifMode.Rest => "/Assets/Animations/rest.gif",
                GifMode.Run => "/Assets/Animations/run.gif",
                GifMode.FastRun => "/Assets/Animations/fastrun.gif",
                _ => currentGifSource
            };
        }

        private void UpdateGifMode(double progress)
        {
            if (IsPaused)
            {
                CurrentGifMode = GifMode.Stay;
            }
            else if (CurrentState == PomodoroState.Rest)
            {
                CurrentGifMode = GifMode.Rest;
            }
            else
            {
                // Change mode based on progress
                CurrentGifMode = progress >= 0.33 && progress <= 0.66 ? GifMode.FastRun : GifMode.Run;
            }
        }

        private void UpdateGifPosition(double progress)
        {
            // Define the maximum position based on window width and GIF width
            const double maxPosition = 275; // 350 (window width) - 75 (GIF width)
            double newPosition;

            // For Rest state, position is fixed
            if (CurrentState == PomodoroState.Rest)
            {
                newPosition = isMovingRight ? maxPosition : 0;
                GifPosition = newPosition;
                IsFlipped = isMovingRight ? false : true;
                return;
            }

            // For Work state, calculate position based on progress and direction
            if (isMovingRight)
            {
                newPosition = progress * maxPosition;
                IsFlipped = false;
            }
            else
            {
                newPosition = (1 - progress) * maxPosition;
                IsFlipped = true;
            }

            // Clamp the position to ensure it stays within bounds
            newPosition = Math.Max(0, Math.Min(maxPosition, newPosition));
            GifPosition = newPosition;
        }

        private double GetProgress()
        {
            return 1.0 - (double)TimerCurrentTime /
                (CurrentState == PomodoroState.Work
                    ? Properties.Settings.Default.work_time
                    : Properties.Settings.Default.rest_time);
        }

        private static void PlayAudioAsync()
        {
            MediaPlayer player = new();
            try
            {
                player.Open(new Uri("Assets/notify_sound.mp3", UriKind.Relative));
                player.Volume = 0.1;
                player.Play();
                player.MediaEnded += (s, e) => player.Close();
            }
            catch (FileNotFoundException)
            {
                System.Windows.MessageBox.Show("File not found");
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("Invalid audio format.");
            }
        }

        private static string SetTimeString(int time)
        {
            return $"{time / 60:00}:{time % 60:00}";
        }

        private void ChangeTheme(object parameter)
        {
            if (parameter is string themeName)
            {
                var selectedTheme = Themes.AvailableThemes.Find(t => t.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase));

                if (selectedTheme != null)
                {
                    _appSettings.SelectedTextColor = selectedTheme.TextColor;
                    _appSettings.SelectedBackgroundImagePath = selectedTheme.BackgroundImagePath;

                    _settingsService.SaveSettings(_appSettings);
                }
            }
        }

        private void OpenCustomColorPicker(object parameter)
        {
            var colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Get selected color and convert to HEX
                var color = colorDialog.Color;
                string hexColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

                // Apply the custom color theme
                _appSettings.SelectedTextColor = hexColor;
                _appSettings.SelectedBackgroundImagePath = "/Assets/Backgrounds/classic/bg_white.png"; // Custom uses white background

                _settingsService.SaveSettings(_appSettings);

                System.Windows.MessageBox.Show($"Custom theme applied with color: {hexColor}");
            }
        }
    }
}