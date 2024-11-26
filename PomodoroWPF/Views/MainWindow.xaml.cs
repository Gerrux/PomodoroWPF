using PomodoroWPF.Services;
using PomodoroWPF.ViewModels;
using PomodoroWPF.Views;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace PomodoroWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            double workHeight = SystemParameters.WorkArea.Height;
            double workWidth = SystemParameters.WorkArea.Width;
            this.Top = (workHeight - this.Height);
            this.Left = (workWidth - this.Width);

            NotifyIcon ni = new NotifyIcon();
            ni.Icon = new System.Drawing.Icon("Assets/Icons/icon.ico");
            ni.Visible = true;
            ni.Text = "PomodoroTimer";
            ni.ContextMenuStrip = new ContextMenuStrip();
            ni.ContextMenuStrip.Items.Add("Settings", null, OnSettingsClicked);
            ni.ContextMenuStrip.Items.Add("Exit", null, OnExitClicked);
            ni.DoubleClick += delegate (object sender, EventArgs args)
            {
                if (WindowState.Equals(WindowState.Minimized))
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.Hide();
                    this.WindowState = WindowState.Minimized;
                }
            };

            // Initialize ViewModel with necessary services
            _viewModel = new MainWindowViewModel(new NotifyIconNotificationService(ni));
            this.DataContext = _viewModel;
        }

        private void OnSettingsClicked(object? sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new();
            settingsWindow.Show();
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            Close();
        }

        #region Draggable window
        protected void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
        #endregion

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
